using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptGen
{
    class General:CompTemplate
    {
        public override string Type { get { return "GEN"; } }
        public override bool IsMainController { get { return false; } }

        public override void WriteScript(CompInfoTemp c, Dictionary<ST, List<int>>scriptNo, ref string scripts)
        {
            //Nothing should be done
        }
    }
}
