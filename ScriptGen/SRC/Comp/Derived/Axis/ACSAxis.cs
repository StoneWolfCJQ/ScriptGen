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

        protected override void WriteHome(CompInfoTemp c, List<int> homeBufferNo, ref string scripts)
        {
            c.contents = (from dict in c.contents
                          from n in dict[KeyWordDef.AN].Split(',').Select(i => int.Parse(i))
                          let ndict=new Dictionary<string, string>(dict)
                          group ndict by n into gdicts orderby gdicts.Key
                          let rdict = ReplaceAxisNo(gdicts.Last(), gdicts.Key)
                          select rdict).ToList();
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

        protected virtual Dictionary<string, string> ReplaceAxisNo(Dictionary<string, string> dict, int axisNo)
        {
            dict[KeyWordDef.AN] = axisNo.ToString();
            return dict;
        }

        protected virtual void CheckAxisNo(CompInfoTemp c)
        {
            if (int.Parse(c.content[KeyWordDef.AN]) >= c.axisOccupied)
            {
                throw new Exception($"{c.rname}轴号{c.content[KeyWordDef.AN]}超限");
            }
        }
    }
}
