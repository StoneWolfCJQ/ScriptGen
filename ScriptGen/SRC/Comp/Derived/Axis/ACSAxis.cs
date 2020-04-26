using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptGen
{
    class ACSAxis:CommonAxis
    {
        public override string Type { get { return "ACS"; } }

        protected override void FillContent(Dictionary<string, string> source, ref CompInfoTemp output)
        {
            output.contents = (from dict in output.contents
                          from n in dict[KeyWordDef.AN].Split(',').Select(i => int.Parse(i))
                          let ndict = new Dictionary<string, string>(dict)
                          group ndict by n into gdict orderby gdict.Key
                          let rdict = AggregateDicts(gdict).Concat(source)
                          .GroupBy(d=>d.Key)
                          .ToDictionary(d=>d.Key, d=>d.First().Value)
                          select rdict).ToList();
            output.content = output.contents[0];
        }

        protected override void WriteHome(CompInfoTemp c, List<int> homeBufferNo, ref string scripts)
        {
            foreach (var content in c.contents)
            {
                c.content = content;
                CheckAxisNo(c);
                base.WriteHome(c, homeBufferNo, ref scripts);
            }
        }

        protected override void WriteComp(CompInfoTemp c, int compBufferNo, List<int> homeBufferNo, ref string scripts)
        {
            foreach (var content in c.contents)
            {
                c.content = content;
                base.WriteComp(c, compBufferNo, homeBufferNo, ref scripts);
            }
        }

        protected override void WriteAuto(CompInfoTemp c, int autoBufferNo, ref string scripts)
        {
            foreach (var content in c.contents)
            {
                c.content = content;
                WriteAutoSingle(c, autoBufferNo, ref scripts);
            }
        }

        protected virtual void WriteAutoSingle(CompInfoTemp c, int autoBufferNo, ref string scripts)
        {

            bool MU = int.Parse(c.content[KeyWordDef.MU]) > 0 ? true : false;
            int index = CompManager.GetBufferIndex(autoBufferNo, scripts);
            int count = CompManager.GetBufferIndex(autoBufferNo, scripts);
            List<Dictionary<string, string>> autoDictList;
            if (MU)
            {
                autoDictList = new List<Dictionary<string, string>>()
                {
                    new Dictionary<string, string>()
                    {
                        {"#AxisNo#", GetAxisNo(c).ToString()},
                    }
                };
                string repeatKeyWord = "CommutRepeat";
                TextFunctions.AppendMultiRepeat(ref scripts, repeatKeyWord, autoDictList, index, count);
            }

            base.WriteAuto(c, autoBufferNo, ref scripts);
        }

        protected virtual Dictionary<string, string> AggregateDicts(IGrouping<int, Dictionary<string, string>> gdict)
        {
            string axisNo = gdict.Key.ToString();
            Dictionary<string, string> ndict = new Dictionary<string, string>();
            foreach (var dt in gdict)
            {
                ndict = ndict.Concat(dt).GroupBy(d => d.Key).ToDictionary(d => d.Key, d => d.Last().Value);
            }
            ndict[KeyWordDef.AN] = axisNo.ToString();
            return ndict;
        }

        protected virtual void CheckAxisNo(CompInfoTemp c)
        {
            if (int.Parse(c.content[KeyWordDef.AN]) >= c.axisOccupied)
            {
                throw new Exception($"{c.rname}轴号{c.content[KeyWordDef.AN]}超限");
            }
        }

        public override void FillContentFromDefLine(CompInfoTemp t, string s)
        {
            Dictionary<string, string> dt = RegFunctions.GetDefLineDict(s);
            t.contents.Add(dt);
        }
    }
}
