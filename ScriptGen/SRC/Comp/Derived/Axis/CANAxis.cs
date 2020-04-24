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
            int index = CompManager.GetBufferIndex(autoBufferNo, scripts);
            int count = CompManager.GetBufferIndex(autoBufferNo, scripts);
            List<Dictionary<string, string>> autoDictList;
            autoDictList =  new List<Dictionary<string, string>>() { c.content.ToDictionary(k => '@' + k.Key, v => v.Value) };
            autoDictList.First().Add("#AxisNo#", c.axisStart.ToString());
            string repeatKeyWord = "CANRepeat";
            TextFunctions.AppendMultiRepeat(ref scripts, repeatKeyWord, autoDictList, index, count);

            base.WriteAuto(c, autoBufferNo, ref scripts);
        }

        protected override int GetAxisNo(CompInfoTemp c)
        {
            return c.axisStart;
        }
    }
}
