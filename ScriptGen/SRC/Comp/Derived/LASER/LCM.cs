using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptGen
{
    class LCM:CompTemplate
    {
        public override string Type { get { return "LCM"; } }
        protected override string ACSStr { get { return "LCM"; } }
        public override bool IsMainController { get { return false; } }

        protected override void WriteHome(CompInfoTemp c, List<int> homeBufferNo, ref string scripts)
        {
            //Nothing should be done
        }

        protected override void WriteComp(CompInfoTemp c, int compBufferNo, List<int> homeBufferNo, ref string scripts)
        {
            //Nothing should be done
        }

        protected override void WriteLaser(CompInfoTemp c, int laserBufferNo, ref string scripts)
        {
            int index = CompManager.GetBufferIndex(laserBufferNo, scripts);
            int count = CompManager.GetBufferCount(laserBufferNo, scripts);

            List<Dictionary<string, string>> LCMDictList = new List<Dictionary<string, string>>()
            {
                new Dictionary<string, string>()
                {
                    {"#AxisNo#", (c.axisStart + 3).ToString()},
                }
            };
            string repeatKeyWord = "LaserRepeat_LCM_" + c.content[KeyWordDef.LM];
            TextFunctions.AppendMultiRepeat(ref scripts, repeatKeyWord, LCMDictList, index, count);
        }

        protected override void WriteAuto(CompInfoTemp c, int autoBufferNo, ref string scripts)
        {
            //Nothing should be done
        }

        protected override void WriteDBuffer(CompInfoTemp c, ref string scripts)
        {
            //Nothing should be done
        }
    }
}
