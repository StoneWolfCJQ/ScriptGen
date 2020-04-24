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
}
