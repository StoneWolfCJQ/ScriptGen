﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
        static List<CompInfoTemp> gTopoList;

        public MainHandler()
        {
            defaultDict = new Dictionary<string, Dictionary<string, string>>();
            gTopoList = new List<CompInfoTemp>();
        }

        public string Handle(string[] input)
        {
            input = ScanMacro(input);
            List<string> inputs = input.ToList();
            for (int m = 0; m < inputs.Count; m++)
            {
                inputs[m] = Regex.Replace(inputs[m], RegFunctions.defLineCommentReg, "");
                if (Regex.IsMatch(inputs[m], RegFunctions.defLineCommentReg) || string.IsNullOrEmpty(inputs[m]))
                {
                    inputs.RemoveAt(m);
                    m--;
                }
            }
            input = inputs.ToArray();
            List<CompInfoTemp> topolist = new List<CompInfoTemp>();
            topolist = ScanTopo(input);
            ScanForCompInfo(input);
            FillTopoFromGeneralCompInfo(topolist, input);
            FillTopoFromUserCompInfo(topolist, input);
            Dictionary<ST, string> STContent = (from kv in STFileName
                                                select new { kv.Key, b = File.ReadAllText(defaultPRGPath + kv.Value) })
                                             .ToDictionary(a=>a.Key, a=>a.b);
            gTopoList = topolist.Concat(gTopoList).ToList();
            string script = CManager.GenerateScript(STContent, gTopoList);
            script = TextFunctions.RemoveInvalidLine(script, DeleteOptions.ALL);
            script = TextFunctions.TrimStrInList(script);
            script = TextFunctions.SubSpecial(script);
            string date = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt", new CultureInfo("en-US"));
            string head = $"#/ Controller version = 2.70\r\n#/ Date = {date}\r\n#/ " +
                $"User remarks = Automatically generated by ScriptGen\r\n";
            script = script.Insert(0, head);
            return script;
        }

        //Scan macro and store
        string[] ScanMacro(string[] input)
        {
            string temp = string.Join("\r\n", input);
            Dictionary<string, string> tcontent = new Dictionary<string, string>()
                {
                    {KeyWordDef.AT, "MACRO" },
                };
            Dictionary<string, List<string>> macroDict = RegFunctions.GetMacroDictAndRemove(temp, out string output);
            foreach (var kv in macroDict)
            {
                CompInfoTemp ct = new CompInfoTemp() { gname = "MACRO", rname = kv.Key, content = tcontent };
                ct.contents = (from mkv in macroDict
                               from value in kv.Value
                               let key = "#ITEM#"
                               select new { key, value })
                               .Select(dict => new Dictionary<string, string> { { dict.key, dict.value } })
                               .ToList();
                gTopoList.Add(ct);
            }
            return output.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        List<CompInfoTemp> ScanTopo(string[] input)
        {
            List<Dictionary<string, string>> strDict = RegFunctions.GetTopoDict(input[0]);
            if (strDict.Count <= 0) 
            {
                throw new Exception("首行不是拓补定义");
            }
            List<CompInfoTemp> topoList = new List<CompInfoTemp>();
            Dictionary<string, int> compMaxNumDict = new Dictionary<string, int>();
            Dictionary<string, List<int>> checkNumDict = new Dictionary<string, List<int>>();
            foreach (var kv in strDict)
            {
                List<int> compNumList = new List<int>();
                string compName = kv.First().Key;
                string sv = kv.First().Value;
                int i;
                DictionaryFunctions.GetValueOrAddNewKey(checkNumDict, compName, new List<int>());
                if (sv.Contains('$'))
                {
                    List<int> li;
                    try
                    {
                        li = sv.Split('$')[1].Split(',').Select(s => int.Parse(s)).ToList();
                    }
                    catch
                    {
                        throw new Exception($"无效值{compName}:{sv}");
                    }
                    int num = int.TryParse(sv.Split('$')[0], out num) ? num : li.Count;
                    for (i = 1; i <= num; i++)
                    {
                        if (i <= li.Count)
                        {
                            compNumList.Add(li[i - 1]);
                        }
                        else
                        {
                            compNumList.Add(li.Last() + i - li.Count);
                        }
                    }
                }
                else
                {
                    int value = int.Parse(kv.First().Value);
                    i = DictionaryFunctions.GetValueOrAddNewKey(compMaxNumDict, compName, 0) + 1;
                    compMaxNumDict[compName] += value;
                    for (; i <= compMaxNumDict[compName]; i++)
                    {
                        compNumList.Add(i);
                    }
                }
                ;
                foreach(int j in compNumList)
                {
                    i = j;
                    if (checkNumDict[compName].Contains(i))
                    {
                        throw new Exception($"重复编号{compName}:{i}");
                    }
                    checkNumDict[compName].Add(i);
                    CompInfoTemp temp = new CompInfoTemp() 
                    {
                        rname = compName + i.ToString(),
                        gname = compName,
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
                            DictionaryFunctions.AddOrUpdatePair(kv, d);
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
            input = (from cc in input
                     let rt = Regex.Match(cc, RegFunctions.compCustomReg)
                     where rt.Success
                     let name = rt.Groups[1].Value
                     let num = rt.Groups[2].Value.Split(',')
                     from n in num
                     select name + n + rt.Groups[4]).ToArray();
            foreach (string s in input)
            {
                CompInfoTemp t = topolist.Find(c => c.rname == s.Split('@')[0]);
                if (t == null)
                {
                    continue;
                }
                string type = defaultDict[t.gname][KeyWordDef.AT];
                if (t != null)
                {
                    CManager.FillContentFromDefLine(t, s, type);
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

        public static List<int> GetAxisNoByCompName(string compList)
        {
            List<string> compNameList = RegFunctions.SplitToListByComma(compList);
            List<int> li = new List<int>();
            foreach (string s in compNameList)
            {
                string n = s.Split('.')[0];
                string i = s.Split('.').Length > 1 ? s.Split('.')[1] : "0";
                try
                {
                    li.Add(gTopoList.Find(c => c.rname == s.Split('.')[0]).axisStart + int.Parse(i));
                }
                catch
                {
                    continue;
                }
            }
            return li;
        }
    }
}
