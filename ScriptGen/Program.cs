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
            MainHandler MH = new MainHandler();
            List<string> paths = args.ToList();
            while (true)
            {
                foreach (string p in paths)
                {
                    if (p== "Q" || p == "q")
                    {
                        return;
                    }
                    string path = p.Trim('"');
                    try
                    {
                        string[] fileStrs = File.ReadAllLines(path);
                        string prg = MH.Handle(fileStrs);
                        string name = Path.GetDirectoryName(path) + '\\' + Path.GetFileNameWithoutExtension(path) + ".prg";
                        if (!File.Exists(name))
                        {
                            File.Create(name).Close();
                        }
                        File.WriteAllText(name, prg);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                paths.Clear();
                paths.Add(Console.ReadLine());
            }
        }
    }
}
