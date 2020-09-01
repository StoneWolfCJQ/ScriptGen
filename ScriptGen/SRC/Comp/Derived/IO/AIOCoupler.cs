using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ScriptGen
{
    class AIOCoupler: IOCoupler
    {
        public override string Type { get { return "AIOC"; } }

        protected override int GetSO(Dictionary<string, string> d, CompInfoTemp output)
        {
            int so = base.GetSO(d, output);
            ParseTopo(output.content.ContainsKey(KeyWordDef.TOP) ? output.content[KeyWordDef.TOP] : d[KeyWordDef.TOP]);
            foreach (var dt in TopoDict)
            {
                so -= dt.First().Value;
            }
            return so;
        }

        protected override List<Dictionary<string, string>> GenerateDictList(CompInfoTemp c, out int IIndex, out int OIndex)
        {
            List<Dictionary<string, string>> IODictList = new List<Dictionary<string, string>>();
            IIndex = GetIOStartIndex(true, c);
            OIndex = GetIOStartIndex(false, c);
            int IOIncrement = 0;
            foreach (var dt in TopoDict)
            {
                int i = dt.First().Value;
                bool isIn = dt.First().Key;
                if (i % 8 != 0)
                {
                    throw new Exception($"部件{c.rname}属性错误：IO数目必须是8的整数倍");
                }
                for (int jj = 0; i > 0; i -= 8, jj++)
                {
                    var l = GenerateSingleDictList(c, isIn, ref IOIncrement, ref IIndex, ref OIndex);
                    IIndex -= isIn ? 1 : 0;
                    OIndex -= !isIn ? 1 : 0;
                    l.RemoveAt(1);
                    l.First()["#NUM#"] = jj.ToString();
                    l.First()["#MappingName#"] = isIn ? "DI1" : "DO1";
                    IOIncrement = 0;
                    IODictList.Add(l.First());
                }
            }
            return IODictList;
        }
    }
}
