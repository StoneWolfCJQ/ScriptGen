﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

namespace ScriptGen
{
    class MainHandler
    {
        Dictionary<string, Dictionary<string, string>> defaultDict;
        string defaultInfoPath = @"DEF\";
        string defaultPRGPath = @"PRG\";
        Dictionary<ST, string> STFileName = new Dictionary<ST, string>
        {
            {ST.HOME, "00-Home.prg" },
            {ST.COMP, "01-Comp.prg" },
            {ST.LASER, "02-Laser.prg" },
            {ST.AUTO, "31-AutoExec.prg" },
            {ST.DEF, "AA-DBuffer.prg" }
        };
        CompManager CManager = new CompManager();

        public MainHandler()
        {
            defaultDict = new Dictionary<string, Dictionary<string, string>>();
        }

        public string Handle(string[] input)
        {
            List<CompInfoTemp> topolist = ScanTopo(input);
            ScanForCompInfo(input);
            FillTopoFromGeneralCompInfo(topolist, input);
            FillTopoFromUserCompInfo(topolist, input);
            Dictionary<ST, string> STContent = (from kv in STFileName
                                                select new { kv.Key, b = File.ReadAllText(defaultPRGPath + kv.Value) })
                                             .ToDictionary(a=>a.Key, a=>a.b);
            string script = CManager.GenerateScript(STContent, topolist);
            script = TextFunctions.RemoveInvalidLine(script, DeleteOptions.ALL);
            script = TextFunctions.TrimComma(script);
            string date = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt", new CultureInfo("en-US"));
            string head = $"#/ Controller version = 2.70\r\n#/ Date = {date}\r\n#/ User remarks = Automatic generated by ScriptGen\r\n";
            script = script.Insert(0, head);
            return script;
        }

        List<CompInfoTemp> ScanTopo(string[] inputs)
        {
            List<string> input = inputs.ToList();
            for (int m = 0; m < input.Count; m++)
            {
                if (Regex.IsMatch(input[m], @"((^\s*(\/\/))|(^\s+$))") || string.IsNullOrEmpty(input[m]))
                {
                    input.RemoveAt(m);
                    m--;
                }
            }
            List<Dictionary<string, string>> strDict = RegFunctions.GetTopoDict(input[0]);
            if (strDict.Count <= 0) 
            {
                throw new Exception("首行不是拓补定义");
            }
            List<CompInfoTemp> topoList = new List<CompInfoTemp>();
            Dictionary<string, int> compNumDict = new Dictionary<string, int>();
            foreach (var kv in strDict)
            {
                int i;
                if (compNumDict.ContainsKey(kv.First().Key))
                {
                    i = compNumDict[kv.First().Key] + 1;
                    compNumDict[kv.First().Key] += int.Parse(kv.First().Value);
                }
                else
                {
                    i = 1;
                    compNumDict.Add(kv.First().Key, int.Parse(kv.First().Value));
                }
                for (; i <= compNumDict[kv.First().Key]; i++) 
                {
                    CompInfoTemp temp = new CompInfoTemp() 
                    {
                        rname = kv.First().Key + i.ToString(),
                        gname = kv.First().Key,
                    };
                    topoList.Add(temp);
                }
            }
            return topoList;
        }

        void ScanForCompInfo(string[] input)
        {
            for (int i = 1; i < input.Length; i++)
            {
                if (RegFunctions.IsCompBaseInfo(input[i]))
                {
                    string s = input[i].Split('@')[0];
                    Dictionary<string, string> d = ScanFile(s);
                    Dictionary<string, string> d2 = RegFunctions.GetDefLineDict(input[i]);
                    defaultDict.Add(s, new Dictionary<string, string>());
                    if (d != null)
                    {
                        foreach (var kv in d2)
                        {
                            DictionaryFunctions.AddPair(kv, d);
                        }
                        defaultDict[s] = d;
                    }
                    else
                    {
                        defaultDict[s] = d2;
                    }
                }
            }
        }

        void FillTopoFromGeneralCompInfo(List<CompInfoTemp> topolist, string[] input)
        {
            for (int i = 0; i < topolist.Count; i++)
            {
                CompInfoTemp c = topolist[i];
                if (!defaultDict.ContainsKey(c.gname))
                {
                    Dictionary<string, string> d = ScanFile(c.gname);
                    if (!DictionaryFunctions.NotNullOrEmpty(d)) 
                    {
                        throw new Exception($"未定义的部件{c.gname}");
                    }
                    defaultDict.Add(c.gname, d);
                }              
            }
        }

        void FillTopoFromUserCompInfo(List<CompInfoTemp> topolist, string[] input)
        {
            foreach (string s in input)
            {
                CompInfoTemp t = topolist.Find(c => c.rname == s.Split('@')[0]);
                if (t != null) 
                {
                    foreach(var kv in RegFunctions.GetDefLineDict(s))
                    {
                        DictionaryFunctions.AddPair(kv, t.content);
                    }
                    t.contents.Add(new Dictionary<string, string>(t.content));
                }
            }
            int slaveIndex = 0;
            int axisIndex = 0;
            for (int i = 0; i < topolist.Count; i++)
            {
                CompInfoTemp c = topolist[i];
                CManager.FillSlaveAndACSAxis(ref slaveIndex, ref axisIndex, defaultDict[c.gname], ref c);
            }

            for (int i = 0; i < topolist.Count; i++)
            {
                CompInfoTemp c = topolist[i];
                CManager.FillAllAxisAndContent(ref axisIndex, defaultDict[c.gname], ref c);
            }
        }

        Dictionary<string, string> ScanFile(string type)
        {
            string filePath = defaultInfoPath + type + ".txt";
            if (File.Exists(filePath))
            {
                string fileStr = File.ReadAllText(filePath);
                return RegFunctions.GetDefaultDict(fileStr);
            }
            else
            {
                return null;
            }
        }
    }
}
