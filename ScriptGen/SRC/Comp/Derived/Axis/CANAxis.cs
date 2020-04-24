using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptGen
{
    class CANAxis:CommonAxis
    {
        public override string Type { get { return "CAN"; } }

        protected override void WriteAuto(CompInfoTemp c, int autoBufferNo, ref string scripts)
        {
            int index = CompManager.GetBufferIndex(autoBufferNo);
            int count = CompManager.GetBufferIndex(autoBufferNo);
            List<Dictionary<string, string>> autoDictList;
            autoDictList = new List<Dictionary<string, string>>()
            {
                new Dictionary<string, string>()
                {
                    {"#AxisNo", GetAxisNo(c).ToString()},
                }
            };
            string repeatKeyWord = "CommutRepeat";
            TextFunctions.AppendMultiRepeat(ref scripts, repeatKeyWord, autoDictList, index, count);
        }

        protected virtual void WriteAutoSingle(CompInfoTemp c, int autoBufferNo, ref string scripts)
        {
            bool MU = int.Parse(c.content[KeyWordDef.MU]) > 1 ? true : false;


        }

        protected virtual Dictionary<string, string> ReplaceAxisNo(Dictionary<string, string> dict, int axisNo)
        {
            dict[KeyWordDef.AN] = axisNo.ToString();
            return dict;
        }

        protected override 
    }
}
