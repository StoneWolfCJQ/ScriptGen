﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ScriptGen
{
    class CommonAxis:CompTemplate
    {
        public override bool IsMainController { get { return false; } }
        protected static List<string> nameList;
        protected static Dictionary<int, List<int>> homeSZDict;

        public CommonAxis()
        {
            nameList = new List<string>();
            homeSZDict = new Dictionary<int, List<int>>();
        }

        protected override void WriteHome(CompInfoTemp c, List<int> homeBufferNo, ref string scripts)
        {
            string CHM = c.content[KeyWordDef.HM];

            if (CHM.Contains("D"))
            {
                return;
            }

            int HG = GetHomeBufferNo(c, homeBufferNo);
            int HGIndex = GetHomeIndex(c, homeBufferNo, scripts);
            int count = GetHomeCount(c, homeBufferNo, scripts);
            TextFunctions.ReplaceSingle(ref scripts, "BH", HG.ToString(), HGIndex, count);

            string HM = "";

            if (CHM.Contains("L"))
            {
                HM += "L";
            }
            else if (CHM.Contains("R"))
            {
                HM += "R";
            }
            else
            {
                throw new Exception("未定义回零方向");
            }

            if (CHM.Contains("I"))
            {
                HM += "I";
            }

            List<string> homeSpeedList = c.content[KeyWordDef.HS].Split(",".ToArray(), 
                StringSplitOptions.RemoveEmptyEntries).ToList();
            List<Dictionary<string, string>> homeDictList = new List<Dictionary<string, string>>()
            {
                new Dictionary<string, string>()
                {
                    {"#AxisNo#", GetAxisNo(c).ToString()},
                    {"#NSpeed#", homeSpeedList[0] },
                    {"#HSpeed#", homeSpeedList.Count > 1 ? homeSpeedList[1] : (int.Parse(homeSpeedList[0])/2).ToString()},
                    {"@HP", c.content.ContainsKey(KeyWordDef.HP)? c.content[KeyWordDef.HP] : "0" },
                    {"@HF", c.content[KeyWordDef.HF] },
                    {"#HomingMethod#", HM },
                    {"#GoSafe#" , c.content.ContainsKey(KeyWordDef.HP)? "" : "!" },
                    {"#NAME#", GetAxisName(c) },
                    {"#COMP#", int.Parse(c.content[KeyWordDef.CN]) > 0 ? "" : "!" },
                }
            };
            string repeatKeyWord = "HomeRepeat";
            TextFunctions.AppendMultiRepeat(ref scripts, repeatKeyWord, homeDictList, HGIndex, count);
           
            if (CHM.Contains("Z") || c.content.ContainsKey(KeyWordDef.SZ))
            {
                TextFunctions.ReplaceSingle(ref scripts, "ZLimitSafeLine__", "", HGIndex, count);
            }

            if (CHM.Contains("Z"))
            {
                homeDictList = new List<Dictionary<string, string>>()
                {
                    new Dictionary<string, string>()
                    {
                        {"#AxisNo#", GetAxisNo(c).ToString()},
                        {"@BH", HG.ToString()},
                    }
                };
                repeatKeyWord = "ZAxisSafeRepeat";
                TextFunctions.AppendMultiRepeat(ref scripts, repeatKeyWord, homeDictList, HGIndex, count);
                repeatKeyWord = "ZLimitSafeRepeat";
                TextFunctions.AppendMultiRepeat(ref scripts, repeatKeyWord, homeDictList, HGIndex, count);
            }

            if (c.content.ContainsKey(KeyWordDef.SZ))
            {
                List<int> li = MainHandler.GetAxisNoByCompName(c.content[KeyWordDef.SZ]);
                if (li.Count == 0)
                {
                    throw new Exception($"部件{c.rname}属性{KeyWordDef.SZ}错误：{c.content[KeyWordDef.SZ]}");
                }
                List<Dictionary<string, string>> SZDictList = new List<Dictionary<string, string>>();
                foreach(int i in li)
                {
                    if (!DictionaryFunctions.GetValueOrAddNewKey(homeSZDict, HG, new List<int>())
                        .Contains(i))
                    {
                        homeSZDict[HG].Add(i);
                        SZDictList.Add(new Dictionary<string, string>()
                        {
                            {"#AxisNo#", i.ToString()},
                            {"@BH", HG.ToString()},
                        });
                    }
                }
                repeatKeyWord = "ZLimitSafeRepeat";
                TextFunctions.AppendMultiRepeat(ref scripts, repeatKeyWord, SZDictList, HGIndex, count);
            }
        }

        protected override void WriteComp(CompInfoTemp c, int compBufferNo, List<int> homeBufferNo, ref string scripts)
        {
            string CHM = c.content[KeyWordDef.HM];

            if (!CHM.Contains("D"))
            {
                TextFunctions.AppendMultiRepeat(
                    ref scripts,
                    "SingleAxisHome",
                    new List<Dictionary<string, string>>(){
                new Dictionary<string, string>()
                    {
                        {"#AxisNo#", GetAxisNo(c).ToString()},
                        {"#NAME#", GetAxisName(c, false) },
                        {"@HG", GetHomeBufferNo(c, homeBufferNo).ToString() },
                    }
                     },
                    CompManager.GetBufferIndex(compBufferNo, scripts),
                    CompManager.GetBufferCount(compBufferNo, scripts)
                    );
            }
            bool CN = int.Parse(c.content[KeyWordDef.CN]) > 0 ? true : false;
            if (!CN)
            {
                return;
            }
            int index = GetHomeIndex(c, homeBufferNo, scripts);
            int count = GetHomeCount(c, homeBufferNo, scripts);

            List<Dictionary<string, string>> compDictList = new List<Dictionary<string, string>>()
            {
                new Dictionary<string, string>()
                {
                    {"#AxisNo#", GetAxisNo(c).ToString()},
                    {"#BCNo#", compBufferNo.ToString()},
                    {"@CS", c.content[KeyWordDef.CS]},
                    {"@CT", c.content[KeyWordDef.CT]},
                    {"@CND", c.content[KeyWordDef.CND] },
                    {"#NAME#", GetAxisName(c, false) },
                    {"@HG", GetHomeBufferNo(c, homeBufferNo).ToString() },
                }
            };
            string repeatKeyWord = "CompRepeat";
            TextFunctions.AppendMultiRepeat(ref scripts, repeatKeyWord, compDictList, index, count);
            index = CompManager.GetBufferIndex(compBufferNo, scripts);
            count = CompManager.GetBufferCount(compBufferNo, scripts);
            TextFunctions.AppendMultiRepeat(ref scripts, repeatKeyWord, compDictList, index, count);
            index = CompManager.GetBufferIndex(ST.DEF, scripts);
            TextFunctions.AppendMultiRepeat(ref scripts, repeatKeyWord, compDictList, index);
        }

        protected virtual string GetAxisName(CompInfoTemp c, bool check = true)
        {
            if (c.content.TryGetValue(KeyWordDef.NAME, out string name))
            {
                if (!Regex.IsMatch(name, @"^\w+$") || Regex.IsMatch(name, @"^Axis\d+$"))
                {
                    throw new Exception($"轴名称非法{name}");
                }
            }
            else
            {
                name = $"Axis{GetAxisNo(c)}";                             
            }

            if (nameList.Contains(name) && check)
            {
                throw new Exception($"轴名称重复{name}");
            }
            nameList.Add(name);
            return name;
        }

        protected override void WriteLaser(CompInfoTemp c, int laserBufferNo, ref string scripts)
        {
            //Nothing should be done
        }

        protected override void WriteAuto(CompInfoTemp c, int autoBufferNo, ref string scripts)
        {
            bool LL, RL;
            LL = c.content.ContainsKey(KeyWordDef.LL);
            RL = c.content.ContainsKey(KeyWordDef.RL);
            if (LL || RL)
            {
                int index = CompManager.GetBufferIndex(autoBufferNo, scripts);
                int count = CompManager.GetBufferIndex(autoBufferNo, scripts);
                List<Dictionary<string, string>> autoDictList = new List<Dictionary<string, string>>()
                {
                    new Dictionary<string, string>()
                    {
                        {"#AxisNo#", GetAxisNo(c).ToString()},
                        {"@RL", RL ? c.content[KeyWordDef.RL] : "#RL" },
                        {"@LL", LL ? c.content[KeyWordDef.LL] : "#LL" },
                        {"#RightLimit#", RL ? "" : "!" },
                        {"#LeftLimit#", LL ? "" : "!" },
                    }
                };
                string repeatKeyWord = "LimitRepeat";
                TextFunctions.AppendMultiRepeat(ref scripts, repeatKeyWord, autoDictList, index, count);
            }
        }

        protected override void WriteDBuffer(CompInfoTemp c, ref string scripts)
        {
            //Nothing should be done
        }

        protected virtual int GetHomeIndex(CompInfoTemp c, List<int> homeBufferNo, string scripts)
        {            
            int HGIndex = CompManager.GetBufferIndex(GetHomeBufferNo(c, homeBufferNo), scripts);
            return HGIndex;
        }

        protected virtual int GetHomeCount(CompInfoTemp c, List<int> homeBufferNo, string scripts)
        {
            int count = CompManager.GetBufferCount(GetHomeBufferNo(c, homeBufferNo), scripts);
            return count;
        }

        protected virtual int GetHomeBufferNo(CompInfoTemp c, List<int> homeBufferNo)
        {
            int HG = int.Parse(c.content[KeyWordDef.HG]);
            if (!homeBufferNo.Contains(HG))
            {
                throw new Exception("回零Buffer号不在定义范围内");
            }
            return HG;
        }

        protected virtual int GetAxisNo(CompInfoTemp c)
        {
            return c.axisStart;
        }
    }
}
