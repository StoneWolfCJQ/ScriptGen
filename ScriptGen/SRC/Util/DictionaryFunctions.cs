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
    static class DictionaryFunctions
    {
        public static bool NotNullOrEmpty<T1, T2>(Dictionary<T1, T2> d)
        {
            return !((d == null) || (d.Keys.Count == 0));
        }

        public static void AddOrUpdatePair<T1,T2>(KeyValuePair<T1,T2> kv, Dictionary<T1,T2> d)
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

        public static void AddPair<T1, T2>(KeyValuePair<T1, T2> kv, Dictionary<T1, T2> d)
        {
            if (!d.ContainsKey(kv.Key))
            {
                d[kv.Key] = kv.Value;
            }
        }

        public static T2 GetValueOrThrowException<T1, T2>(string info, Dictionary<T1, T2> d, T1 key)
        {
            if (d.ContainsKey(key))
            {
                return d[key];
            }

            throw new Exception($"无法获取{info}键值：{key}");
        }

        public static T2 GetValueOrAddNewKey<T1, T2>(Dictionary<T1, T2> d, T1 key, T2 newValue)
        {
            try
            {
                GetValueOrThrowException("", d, key);
            }
            catch
            {
                d.Add(key, newValue);
            }
            return d[key];
        }
    }
}
