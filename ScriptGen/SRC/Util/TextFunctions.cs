using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

namespace ScriptGen
{
    static class TextFunctions
    {
        public static Dictionary<string, string> subStrList = new Dictionary<string, string>()
        {
            {"$","&" },
        };

        public static List<string> trimList = new List<string>()
        {
            ",",
            @"\$"
        };

        public static void ReplaceSingle(ref string source, Dictionary<string, string> map,
            int startIndex = 0, int count = int.MaxValue)
        {
            foreach (var kv in map)
            {
                ReplaceSingle(ref source, kv.Key, kv.Value, startIndex, count);
            }
        }

        public static void ReplaceSingle(ref string source, string oldStr, string newStr, 
            int startIndex = 0, int count = int.MaxValue)
        {
            count = Utils.GetCount(source, startIndex, count);
            string s = source.Substring(startIndex, count);
            s = s.Replace("@@" + oldStr, newStr);
            source = source.Substring(0, startIndex) + s + 
                source.Substring(startIndex + count, source.Length - startIndex - count);
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
                else if (c == '@' && source[i + 1] != '@' && source[i - 1] != '@')
                {
                    string s = "";
                    for (i++; i < source.Length; i++)
                    {
                        if (IsReturn(source[i])) 
                        {
                            if (s.IndexOf(oldStr) != -1)
                            {
                                string sp = "";
                                string wl = GetWholeLine(source, i);
                                foreach (string ns in newStr.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries))
                                {
                                    sp += wl.Replace('@' + oldStr, ns) + "\r\n";
                                }
                                source = source.Insert(i + 2, sp);
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
            Dictionary<int, string> ms = RegFunctions.GetRepeat(source, keyWord, startIndex, count);
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
                    t.RemoveAt(t.Count - 1);
                    matchStr = string.Join("\r\n", t);
                    source = source.Insert(index, matchStr);
                    index += matchStr.Length;
                }
                index -= rkv.Key;
            }
        }

        public static string RemoveInvalidLine(string input, DeleteOptions dop)
        {
            string ATReg = @"\@";
            string HASHReg = @"\#\w+\#";
            string pattern;
            List<string> patterns = new List<string>();
            if (((dop & DeleteOptions.AND) > 0))
            {
                input = RemoveRepeat(input);
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
                    inputs.RemoveAt(i);
                    i--;
                }
            }
            string output = string.Join("\r\n", inputs);
            return output;
        }

        static string RemoveRepeat(string input)
        {
            input = Regex.Replace(input, "&" + @"\w{2,}" + RegFunctions.repeatRegTail, "");
            return input;
        }

        public static string TrimStrInList(string input)
        {
            foreach (string s in trimList)
            {
                input = Regex.Replace(input, s + @"(?=\s*(\r|\n))", "");
            }
            return input;
        }

        public static string SubSpecial(string input)
        {
            foreach (var kv in subStrList)
            {
                input = input.Replace(kv.Key, kv.Value);
            }
            return input;
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
