using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptGen
{
    class MACRO: CompTemplate
    {
        public override string Type { get { return "MACRO"; } }
        public override bool IsMainController { get { return false; } }

        public override void WriteScript(CompInfoTemp c, Dictionary<ST, List<int>>scriptNo, ref string scripts)
        {
            TextFunctions.AppendMultiRepeat(ref scripts, c.rname, c.contents);
        }
    }
}
