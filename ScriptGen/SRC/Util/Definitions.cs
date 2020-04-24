using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

namespace ScriptGen
{
    /// <summary>
    /// Script type enum
    /// </summary>
    enum ST
    {
        HOME,
        COMP,
        LASER,
        AUTO,
        DEF
    }

    enum DeleteOptions
    {
        AT=1,
        HASHTAG=2,
        AND=4,
        ALL=7
    }

    class CompInfoTemp
    {
        public string rname;//udm1 etc
        public string gname;//udm etc
        public int axisStart;
        public int axisOccupied;
        public int slaveStart;
        public int slaveOccupied;
        public Dictionary<string, string> content;
        public List<Dictionary<string, string>> contents;

        public CompInfoTemp()
        {
            content = new Dictionary<string, string>();
            contents = new List<Dictionary<string, string>>();
        }
    }
}
