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
        static void Main(string[] args)
        {
            foreach (string ss in args)
            {
                Console.WriteLine(ss);
            }
            Console.ReadKey();
            MainHandler mh = new MainHandler();
            string[] s = File.ReadAllLines("1.txt");
            string r = mh.Handle(s);
            string name = "scripts.prg";
            if (!File.Exists(name))
            {
                File.Create(name);
            }
            File.WriteAllText(name, r);
        }
    }
}
