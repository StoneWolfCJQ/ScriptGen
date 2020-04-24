using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ScriptGen
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MainHandler mh = new MainHandler();
            string[] s = File.ReadAllLines("1.txt");
            mh.Handle(s);
            //string ss = File.ReadAllText("1.txt");
            //string keyword = "IOINRepeat";
            //List<Dictionary<string, string>> replaceDictList = new List<Dictionary<string, string>>()
            //{
            //    new Dictionary<string, string>()
            //    {
            //        {"#SlaveIndex#","1"},
            //        {"@INAME","EIN" },
            //        {"#IOIndexA#","1"},
            //        {"#IOIndexB#","2"}
            //    },
            //    new Dictionary<string, string>()
            //    {
            //        {"#SlaveIndex#","2"},
            //        {"@INAME","EIN" },
            //        {"#IOIndexA#","3"},
            //        {"#IOIndexB#","4"}
            //    }
            //};
            //TextFunctions.AppendMultiRepeat(ref ss, keyword, replaceDictList, 5);
            //ss = TextFunctions.RemoveInvalidLine(ss, DeleteOptions.ALL);
        }
    }
}
