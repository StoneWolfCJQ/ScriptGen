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
    static class RegFunctions
    {
        public const string defaultReg = @"(?<=^|\r\n)(\w{2,})\:([^(\@\/\s)]+)(?=(((\/\/)+(\d\D)*)|\s*|$))";
        public const string defLineReg = @"(?<=\@)(\w{2,})\:([^\@\/\s]+)(?=\@|$)";
        public const string topoReg = @"(?<=\@)(\w{2,})\:?([^\@\/\s]+)?(?=\@|$)";
        public const string compBaseInfoReg = @"^(?i)[a-z]{2,}\@";
        public const string repeatRegTail = @"\s*[\r|\n][^&]+[\r|\n]\s*&\s*?\r\n";

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
}
