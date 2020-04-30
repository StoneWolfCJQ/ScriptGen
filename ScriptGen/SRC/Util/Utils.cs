using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptGen
{
    static class Utils
    {
        public static int GetCount(string input, int startIndex, int count)
        {
            count = count > input.Length - startIndex ? input.Length - startIndex : count;
            return count;
        }
    }
}
