using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace ScriptGen
{
    /// <summary>
    /// Script type enum
    /// </summary>
    enum ST
    {
        HOME,
        COMP,
        LASER,
        AUTO,
        DEF
    }

    enum DeleteOptions
    {
        AT=1,
        HASHTAG=2,
        AND=4,
        ALL=7
    }

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

        public void Handle(string[] input)
        {
            List<CompInfoTemp> topolist = ScanTopo(input);
            ScanForCompInfo(input);
            FillTopoFromGeneralCompInfo(topolist, input);
            FillTopoFromUserCompInfo(topolist, input);
            Dictionary<ST, string> STContent = (from kv in STFileName
                                                select new { kv.Key, b = File.ReadAllText(defaultPRGPath + kv.Value) })
                                             .ToDictionary(a=>a.Key, a=>a.b);
            string script = CManager.GenerateScript(STContent, topolist);
        }

        List<CompInfoTemp> ScanTopo(string[] input)
        {
            List<Dictionary<string, string>> strDict = RegFunction.GetTopoDict(input[0]);
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
                if (RegFunction.IsCompBaseInfo(input[i]))
                {
                    string s = input[i].Split('@')[0];
                    Dictionary<string, string> d = ScanFile(s);
                    Dictionary<string, string> d2 = RegFunction.GetDefLineDict(input[i]);
                    defaultDict.Add(s, new Dictionary<string, string>());
                    if (d != null)
                    {
                        foreach (var kv in d2)
                        {
                            DictionaryFunction.AddPair(kv, d);
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
                    if (!DictionaryFunction.NotNullOrEmpty(d)) 
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
                    foreach(var kv in RegFunction.GetDefLineDict(s))
                    {
                        DictionaryFunction.AddPair(kv, t.content);
                    }
                    t.contents.Add(t.content);
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
                return RegFunction.GetDefaultDict(fileStr);
            }
            else
            {
                return null;
            }
        }
    }

    class CompInfoTemp
    {
        public string rname;//udm1 etc
        public string gname;//udm etc
        public int axisStart;
        public int axisOccupied;
        public int slaveStart;
        public int slaveOccupied;
        public Dictionary<string, string> content;
        public List<Dictionary<string, string>> contents;

        public CompInfoTemp()
        {
            content = new Dictionary<string, string>();
            contents = new List<Dictionary<string, string>>();
        }
    }

    static class RegFunction
    {
        public const string defaultReg = @"(?<=^|\r\n)(\w{2,})\:([^(\@\/\s)]+)(?=(((\/\/)+(\d\D)*)|\s*|$))";
        public const string defLineReg = @"(?<=\@)(\w{2,})\:([^\@\/\s]+)(?=\@|$)";
        public const string topoReg = @"(?<=\@)(\w{2,})\:?([^\@\/\s]+)?(?=\@|$)";
        public const string compBaseInfoReg = @"^(?i)[a-z]{2,}\@";
        public const string repeatRegTail = @"\s*[\r|\n][^&]+[\r|\n]\s*&\s*?[\r|\n]";

        public static Dictionary<string, string> GetDictFromReg(string input, string pattern)
        {
            MatchCollection mc = Regex.Matches(input, pattern);
            Dictionary<string, string> td = new Dictionary<string, string>();
            foreach (Match m in mc)
            {
                td.Add(m.Groups[1].Value, m.Groups[2].Value);
            }
            return td;
        }

        public static Dictionary<string, string> GetDefaultDict(string fileStr)
        {
            return GetDictFromReg(fileStr, defaultReg);
        }

        public static Dictionary<string, string> GetDefLineDict(string input)
        {
            return GetDictFromReg(input, defLineReg);
        }

        public static List<Dictionary<string, string>> GetTopoDict(string input)
        {
            MatchCollection mc = Regex.Matches(input, topoReg);
            List<Dictionary<string, string>> td = new List<Dictionary<string, string>>();
            foreach (Match m in mc)
            {
                td.Add(new Dictionary<string, string>()
                {
                    {
                        m.Groups[1].Value, string.IsNullOrEmpty(m.Groups[2].Value) ? "1" : m.Groups[2].Value
                    }
                });
            }
            return td;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns> Dictionaries of which key is index, value is match string</returns>
        public static Dictionary<int, string> GetRepeat(string input, string keyWord,
            int startIndex = 0, int count = int.MaxValue)
        {
            count = count > input.Length - startIndex ? input.Length - startIndex : count;
            input = input.Substring(startIndex, count);
            MatchCollection mc = Regex.Matches(input, "&" + keyWord + repeatRegTail);
            Dictionary<int, string> result = new Dictionary<int, string>();
            foreach (Match m in mc)
            {
                result.Add(m.Index + startIndex, m.Value);
            }
            return result;
        }

        public static bool IsCompBaseInfo(string input)
        {
            bool b = Regex.IsMatch(input, compBaseInfoReg);
            return b;
        }
    }

    static class DictionaryFunction
    {
        public static bool NotNullOrEmpty<T1, T2>(Dictionary<T1, T2> d)
        {
            return !((d == null) || (d.Keys.Count == 0));
        }

        public static void AddPair<T1,T2>(KeyValuePair<T1,T2> kv, Dictionary<T1,T2> d)
        {
            if (d.ContainsKey(kv.Key))
            {
                d[kv.Key] = kv.Value;
            }
            else
            {
                d.Add(kv.Key, kv.Value);
            }
        }

        public static void AddOnlyNewPair<T1, T2>(KeyValuePair<T1, T2> kv, Dictionary<T1, T2> d)
        {
            if (!d.ContainsKey(kv.Key))
            {
                d[kv.Key] = kv.Value;
            }
        }

        public static T2 GetValue<T1,T2>(Dictionary<T1, T2>d, T1 key, out bool keyExists)
        {
            if (d.ContainsKey(key))
            {
                keyExists = true;
                return d[key];
            }

            keyExists = false;
            return (T2)Convert.ChangeType(null, typeof(T2));
        }
    }

    static class TextFunctions
    {
        public static void ReplaceSingle(ref string source, Dictionary<string, string> map, int startIndex = 0)
        {
            foreach (var kv in map)
            {
                ReplaceSingle(ref source, kv.Key, kv.Value, startIndex);
            }
        }

        public static void ReplaceSingle(ref string source, string oldStr, string newStr, int startIndex = 0)
        {
            string s = source.Substring(startIndex);
            s = s.Replace("*@@" + oldStr, newStr);
            s = s.Replace("@@" + oldStr, newStr);
            source = source.Substring(0, startIndex) + s;
        }

        public static void AppendMultiNoRepeat(ref string source, Dictionary<string, string> map, int startIndex = 0)
        {
            foreach (var kv in map)
            {
                AppendMultiNoRepeat(ref source, kv.Key, kv.Value, startIndex);
            }
        }

        public static void AppendMultiNoRepeat(ref string source, string oldStr, string newStr, int startIndex = 0)
        {
            for (int i = startIndex; i < source.Length; i++)
            {
                char c = source[i];
                if (c == '&')
                {
                    for (i++; i < source.Length; i++)
                    {
                        c = source[i];
                        if (c == '&') break;
                    }
                    if (i == source.Length)
                    {
                        throw new Exception("脚本的&符号不平衡");
                    }
                    continue;
                }
                else if ((c == '@') && (source[i + 1] != '@'))
                {
                    string s = "";
                    for (i++; i < source.Length; i++)
                    {
                        if (IsReturn(source[i])) 
                        {
                            if (s.IndexOf(oldStr) != -1)
                            {
                                string sp = GetWholeLine(source, i).Replace('@' + oldStr, newStr);
                                source = source.Insert(i + 2, sp + "\r\n");
                            }
                            break;
                        }
                        else if (source[i] == '&')
                        {
                            throw new Exception("@与&不能在同一行");
                        }
                        s += source[i];
                    }
                }
            }
        }

        public static void AppendMultiRepeat(ref string source, string keyWord,
            List<Dictionary<string, string>> replaceDictList, 
            int startIndex = 0, int count= int.MaxValue)
        {
            Dictionary<int, string> ms = RegFunction.GetRepeat(source, keyWord, startIndex, count);
            int index = 0;
            foreach (var rkv in ms)
            {
                index += rkv.Key;
                foreach(var dt in replaceDictList)
                {
                    string matchStr = rkv.Value;
                    foreach (var tkv in dt)
                    {
                        matchStr = matchStr.Replace(tkv.Key, tkv.Value);
                    }
                    List<string> t = matchStr.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
                    t.RemoveAll(s => s.Contains("&"));
                    matchStr = string.Join("\r\n", t);
                    source = source.Insert(index, matchStr);
                    index += matchStr.Length;
                }
                index -= rkv.Key;
            }
        }

        public static string RemoveInvalidLine(string input, DeleteOptions dop)
        {
            string ATReg = @"(?:[^*\@])\@";
            string ANDReg = @"\&";
            string HASHReg = @"\#\w+\#";
            string pattern;
            List<string> patterns = new List<string>();
            if (((dop & DeleteOptions.AND) > 0))
            {
                patterns.Add(ANDReg);
            }
            if (((dop & DeleteOptions.AT) > 0))
            {
                patterns.Add(ATReg);
            }
            if (((dop & DeleteOptions.HASHTAG) > 0))
            {
                patterns.Add(HASHReg);
            }
            pattern = string.Join("|", patterns);

            List<string> inputs = input.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
            for (int i = 0; i < inputs.Count; i++)
            {
                if (Regex.IsMatch(inputs[i], pattern))
                {
                    if (Regex.IsMatch(inputs[i], ANDReg) && ((dop & DeleteOptions.AND) > 0))
                    {
                        inputs[i] = inputs[i].Substring(0, inputs[i].IndexOf('&'));
                        for (i++; i < inputs.Count; i++)
                        {                            
                            if (Regex.IsMatch(inputs[i], ANDReg))
                            {
                                break;
                            }
                            inputs.RemoveAt(i);
                            i--;
                        }
                    }
                    inputs.RemoveAt(i);
                    i--;
                }
            }
            string output = string.Join("\r\n", inputs);
            return output;
        }

        public static string TrimComma(string input)
        {
            string s = Regex.Replace(input, @",(?=\s*(\r|\n))", "");
            return s;
        }

        static string GetWholeLine(string source, int index)
        {
            string line = "";
            for (int i = index - 1; i >= 0; i--)
            {
                if (IsReturn(source[i]))
                {
                    for (i++; i < source.Length; i++)
                    {
                        if (IsReturn(source[i]))
                        {
                            return line;
                        }
                        line += source[i];
                    }
                }
            }

            throw new Exception("文件格式错误，找不到换行符");
        }

        static bool IsReturn(char c)
        {
            return c == '\r' || c == '\n' || c == '\0';
        }
    }
}
