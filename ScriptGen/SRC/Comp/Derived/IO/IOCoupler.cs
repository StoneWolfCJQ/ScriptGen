using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Runtime.Remoting.Messaging;

namespace ScriptGen
{
    class IOCoupler:CompTemplate
    {
        public override string Type { get { return "IOC"; } }
        public override bool IsMainController { get { return false; } }
        protected virtual string TopoReg { get { return @"(?i)([io])(\d+)"; } }
        protected virtual List<Dictionary<bool ,int>> TopoDict { get; set; }

        public static List<int> inputIndexes;
        public static List<int> outputIndexes;

        public IOCoupler()
        {
            inputIndexes = new List<int>() { -1 } ;
            outputIndexes = new List<int>() { -1 } ;
        }

        protected override int GetSO(Dictionary<string, string> d, CompInfoTemp output)
        {
            int so = base.GetSO(d, output);
            ParseTopo(output.content.ContainsKey(KeyWordDef.TOP) ? output.content[KeyWordDef.TOP] : d[KeyWordDef.TOP]);
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
            string KeyWord = "IORepeat";
            int startIndex = CompManager.GetBufferIndex(ST.AUTO, scripts);
            List<Dictionary<string, string>> IODictList = GenerateDictList(c, out int IIndex, out int OIndex);
            IODictList = IODictList.OrderByDescending(d => d["#IOType#"]).Select((d, i) =>
            { 
                if (i != 0)
                {
                    d.Remove("#StartIndex#");
                }
                return d;
            }).ToList();
            CheckIndexes(IODictList);
            TextFunctions.AppendMultiRepeat(ref scripts, KeyWord, IODictList, startIndex);

            Dictionary<string, string> OADict = new Dictionary<string, string>
            {
                {"OA1",  "BeforeUnstopRepeat"},
                {"OA2",  "DuringEMGRepeat"},
                {"OA3",  "EscapeEMGRepeat"},
            };

            foreach (var kv in OADict)
            {
                if (c.content.ContainsKey(kv.Key))
                {
                    KeyWord = kv.Value;
                    IODictList = GetOADictList(c, kv.Key);
                    TextFunctions.AppendMultiRepeat(ref scripts, KeyWord, IODictList, startIndex);
                }
            }

            if (!c.content.ContainsKey("EMG"))
            {
                c.content.Add("EMG", "EMG");
            }
            TextFunctions.ReplaceSingle(ref scripts, c.content, startIndex);
            TextFunctions.AppendMultiNoRepeat(ref scripts, c.content, startIndex);
        }

        protected virtual List<Dictionary<string, string>> GetOADictList(CompInfoTemp c, string OAKey)
        {
            try
            {
                var v = c.content[OAKey]
                    .Split(',')
                    .Select(s => new Dictionary<string, string>()
                    {
                        {"#Item#", s.Split(':')[0] },
                        {"#Value#", s.Split(':')[1]},
                    })
                    .ToList();
                return v;
            }
            catch(Exception e)
            {
                throw new Exception($"{c.rname}@{OAKey}:{c.content[OAKey]}表达式错误: {e.Message}");
            }
        }

        protected virtual List<Dictionary<string, string>> GenerateDictList(CompInfoTemp c, out int IIndex, out int OIndex)
        {
            List<Dictionary<string, string>> IODictList = new List<Dictionary<string, string>>();
            IIndex = GetIOStartIndex(true, c);
            OIndex = GetIOStartIndex(false, c);
            int IOIncrement = 0;
            foreach (var dt in TopoDict)
            {
                int i = dt.First().Value;
                bool isIn = dt.First().Key;
                for (; i > 0; i--)
                {
                    IODictList.AddRange(GenerateSingleDictList(c, isIn, ref IOIncrement, ref IIndex, ref OIndex));
                }
            }
            return IODictList;
        }

        public virtual List<Dictionary<string, string>> GenerateSingleDictList(CompInfoTemp c, bool isIn, ref int IOIncrement, ref int IIndex, ref int OIndex)
        {
            string MapCommand = isIn ? "ECIN" : "ECOUT";
            string IOType = isIn ? "1" : "0";
            string MappingName = isIn ? "Input" : "Output";
            string IOName = isIn ? c.content[KeyWordDef.INAME] : c.content[KeyWordDef.ONAME];
            int _IONameIndex = isIn ? IIndex : OIndex;
            int _IOIncrement = IOIncrement;
            IIndex += isIn ? 2 : 0;
            OIndex += !isIn ? 2 : 0;
            IOIncrement++;
            return new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                        {
                            {"#StartIndex#",(c.slaveStart + 1).ToString() },
                            {"#MapCommand#",MapCommand },
                            {"#MappingName#", MappingName },
                            {"#Increment#", _IOIncrement.ToString() },
                            {"#IOType#", IOType },
                            {"#NUM#", "0" },
                            {"@IONAME", IOName},
                            {"#IOIndex#", _IONameIndex.ToString() },
                        },
                new Dictionary<string, string>
                        {
                            {"#StartIndex#",(c.slaveStart + 1).ToString() },
                            {"#MapCommand#",MapCommand },
                            {"#MappingName#", MappingName },
                            {"#Increment#", _IOIncrement.ToString() },
                            {"#IOType#", IOType },
                            {"#NUM#", "1" },
                            {"@IONAME", IOName},
                            {"#IOIndex#", (++_IONameIndex).ToString() },
                        },
            };
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
                return isIn ? inputIndexes.Max() + 1 : outputIndexes.Max() + 1;
            }
            else
            {
                return int.Parse(s);
            }
        }

        protected virtual void CheckIndexes(List<Dictionary<string, string>> IODictList)
        {
            CheckIndexes(IODictList, inputIndexes, true);
            CheckIndexes(IODictList, outputIndexes, false);
        }

        protected virtual void CheckIndexes(List<Dictionary<string, string>> IODictList, List<int> indexes, bool isIn)
        {
            indexes.AddRange(from d in IODictList where d["#IOType#"] == (isIn ? "1" : "0") select int.Parse(d["#IOIndex#"]));
            string name = isIn ? "EIN" : "EOUT";
            var count = from i in indexes group i by i into gi where gi.Count() > 1 select new { s = $"{name}{gi.Key}" };
            if (count.Count() > 0)
            {
                throw new Exception("存在重复IO： " + string.Join(", ", from c in count select c.s));
            }
        }
    }
}
