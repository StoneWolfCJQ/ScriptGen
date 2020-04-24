using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptGen
{
    class EC:CompTemplate
    {
        public override string Type { get { return "EC"; } }
        public override bool IsMainController { get { return true; } }

        protected override void WriteHome(CompInfoTemp c, List<int> homeBufferNo, ref string scripts)
        {
            //Nothing should be done
        }

        protected override void WriteComp(CompInfoTemp c, int compBufferNo, ref string scripts)
        {
            //Nothing should be done
        }

        protected override void WriteLaser(CompInfoTemp c, int laserBufferNo, ref string scripts)
        {
            //Nothing should be done
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
