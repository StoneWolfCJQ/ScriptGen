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
            List<string> paths = args.ToList();
            while (true)
            {
                foreach (string p in paths)
                {
                    MainHandler MH = new MainHandler();
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
                            File.WriteAllText(name, prg);
                        }
                        else
                        {
                            while (true)
                            {
                                Console.Write($"文件{name}已存在，是否覆盖（y/n）:");
                                string input = Console.ReadLine();
                                if (input == "y" || input == "Y")
                                {
                                    File.WriteAllText(name, prg);
                                    Console.WriteLine($"写入{name}成功");
                                    break;
                                }
                                else if (input == "N" || input == "n")
                                {
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("无效输入");
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                paths.Clear();
                Console.WriteLine("\r\n请将单个文件拖入");
                paths.Add(Console.ReadLine());
            }
        }
    }
}
