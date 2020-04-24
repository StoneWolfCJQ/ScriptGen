using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ScriptGen
{
    class IOCoupler:CompTemplate
    {
        public override string Type { get { return "IOC"; } }
        public override bool IsMainController { get { return false; } }
        protected virtual string TopoReg { get { return @"(?i)([io])(\d+)"; } }
        protected virtual List<Dictionary<bool ,int>> TopoDict { get; set; }

        protected static int inputStartIndex = 0;
        protected static int outputStartIndex = 0;

        protected override int GetSO(Dictionary<string, string> d, CompInfoTemp output)
        {
            int so = base.GetSO(d, output);
            ParseTopo(output.content[KeyWordDef.TOP]);
            foreach (var dt in TopoDict)
            {
                so += dt.First().Value;
            }
            return so;
        }

        protected virtual void ParseTopo(string topo)
        {
            TopoDict = new List<Dictionary<bool, int>>();
            MatchCollection mc = Regex.Matches(topo, TopoReg);
            foreach (Match m in mc)
            {
                TopoDict.Add(new Dictionary<bool, int>() { { IsIn(m.Groups[1].Value), int.Parse(m.Groups[2].Value)} });
            }                     
        }

        protected virtual string GetIOName(string IorO, Dictionary<string, string> d)
        {
           
            return GetIOName(IsIn(IorO), d);
        }

        protected virtual string GetIOName(bool isIn, Dictionary<string, string> d)
        {
            return isIn ? d[KeyWordDef.INAME] : d[KeyWordDef.ONAME];
        }

        protected virtual bool IsIn(string IorO)
        {
            bool b;
            if (IorO == "I")
            {
                b = true;
            }
            else if (IorO == "O")
            {
                b = false;
            }
            else
            {
                throw new Exception("IO拓扑非法");
            }

            return b;
        }

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
            //Nothing should be done
        }

        protected override void WriteAuto(CompInfoTemp c, int autoBufferNo, ref string scripts)
        {
            ParseTopo(c.content[KeyWordDef.TOP]);
            //Write IO Config
            string IKeyWord = "IOINRepeat";
            string OKeyWord = "IOOUTRepeat";
            List<Dictionary<string, string>> IDictList = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> ODictList = new List<Dictionary<string, string>>();
            int slaveIndex = c.slaveStart + 1;
            int IIndex = GetIOStartIndex(true, c);
            int OIndex = GetIOStartIndex(false, c);
            foreach (var dt in TopoDict)
            {
                int i = dt.First().Value;
                bool isIn = dt.First().Key;
                for (; i > 0; i--)
                {
                    if (isIn)
                    {
                        IDictList.Add(new Dictionary<string, string>
                        {
                            {"#SlaveIndex#",slaveIndex.ToString() },
                            {"@INAME",c.content[KeyWordDef.INAME] },
                            {"#IOIndexA#",IIndex.ToString()},
                            {"#IOIndexB#",(IIndex+1).ToString() }
                        });
                        IIndex += 2;
                        inputStartIndex = IIndex;
                    }
                    else
                    {
                        ODictList.Add(new Dictionary<string, string>
                        {
                            {"#SlaveIndex#",slaveIndex.ToString() },
                            {"@ONAME",c.content[KeyWordDef.ONAME] },
                            {"#IOIndexA#",OIndex.ToString()},
                            {"#IOIndexB#",(OIndex+1).ToString() }
                        });
                        OIndex += 2;
                        outputStartIndex = OIndex;
                    }
                    slaveIndex++;
                }
            }
            int startIndex = CompManager.GetBufferIndex(ST.AUTO, scripts); // scripts.IndexOf($"#{autoBufferNo}\r\n");
            int j= scripts.IndexOf($"#{autoBufferNo}\r\n");
            TextFunctions.AppendMultiRepeat(ref scripts, IKeyWord, IDictList, startIndex);
            TextFunctions.AppendMultiRepeat(ref scripts, OKeyWord, ODictList, startIndex);
            if (!c.content.ContainsKey("EMG"))
            {
                c.content.Add("EMG", "EMG");
            }
            TextFunctions.ReplaceSingle(ref scripts, c.content, startIndex);
            TextFunctions.AppendMultiNoRepeat(ref scripts, c.content, startIndex);
        }

        protected override void WriteDBuffer(CompInfoTemp c, ref string scripts)
        {
            //Nothing should be done
        }

        protected virtual int GetIOStartIndex(bool isIn, CompInfoTemp c)
        {
            string s = c.content[isIn ? KeyWordDef.ISI : KeyWordDef.OSI];
            if (s == "AUTO")
            {
                return isIn ? inputStartIndex : outputStartIndex;
            }
            else
            {
                return int.Parse(s);
            }
        }
    }
}
