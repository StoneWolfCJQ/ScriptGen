using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptGen
{
    class CompManager
    {
        List<CompTemplate> compTeltList = new List<CompTemplate>
        {
            new IOCoupler(),
            new EC(),
            new ACSAxis(),
            new CANAxis(),
            new LCM(),
            new General(),
            new MACRO(),
        };

        static Dictionary<ST, List<int>> scriptNo;
        public const string RNReturnStr = "\r\n";
        public const string bufferSearchStr = "!ForSearchUseDoNotDuplicate";

        public void FillSlaveAndACSAxis(ref int slaveIndex, ref int axisIndex,
            Dictionary<string, string> source, ref CompInfoTemp output)
        {
            GetCompFromType(source[KeyWordDef.AT])
                .FillSlaveAndACSAxis(ref slaveIndex, ref axisIndex, source, ref output);
        }

        public void FillAllAxisAndContent(ref int axisIndex, Dictionary<string, string> source, ref CompInfoTemp output)
        {
            GetCompFromType(source[KeyWordDef.AT]).FillAllAxisAndContent(ref axisIndex, source, ref output);
        }

        public void FillContentFromDefLine(CompInfoTemp t, string defLine, string type)
        {
            GetCompFromType(type).FillContentFromDefLine(t, defLine);
        }

        public string GenerateScript(Dictionary<ST, string> STContent, List<CompInfoTemp> topoList)
        {
            if (!CheckMainController(topoList[0]))
            {
                throw new Exception("第一个部件不是EC");
            }
            string baseScripts = GenerateBase(STContent, topoList[0]);
            for (int i = 0; i < topoList.Count; i++) 
            {
                CompInfoTemp c = topoList[i];
                if (CheckMainController(c) && i > 0)
                {
                    throw new Exception("存在多个EC");
                }
                GetCompFromType(c.content[KeyWordDef.AT]).WriteScript(c, scriptNo, ref baseScripts);
            }

            return baseScripts;
        }

        string GenerateBase(Dictionary<ST, string> STContent, CompInfoTemp c)
        {
            scriptNo = new Dictionary<ST, List<int>>();
            List<int> homeBuffer = c.content[KeyWordDef.BH].Split(",".ToArray(),StringSplitOptions.RemoveEmptyEntries)
                .Select(s=>int.Parse(s)).ToList();
            int compBuffer = int.Parse(c.content[KeyWordDef.BC]);
            int laserBuffer = int.Parse(c.content[KeyWordDef.BL]);
            int autoBuffer = int.Parse(c.content[KeyWordDef.BA]);
            string ss = "";            
            List<int> li = new List<int>();
            foreach (int i in homeBuffer)
            {
                ss += ContraScript(i, STContent, ST.HOME);
                li.Add(i);
            }
            scriptNo.Add(ST.HOME, new List<int>(li));

            ss += ContraScript(compBuffer, STContent, ST.COMP);
            scriptNo.Add(ST.COMP, new List<int>() { compBuffer });
            li.Add(compBuffer);

            ss += ContraScript(laserBuffer, STContent, ST.LASER);
            scriptNo.Add(ST.LASER, new List<int>() { laserBuffer });
            li.Add(laserBuffer);

            ss += ContraScript(autoBuffer, STContent, ST.AUTO);
            scriptNo.Add(ST.AUTO, new List<int>() { autoBuffer });
            li.Add(autoBuffer);

            CheckBufferNo(li);

            ss += ContraScript(999, STContent, ST.DEF);
            scriptNo.Add(ST.DEF, new List<int>() { 999 });
            return ss;
        }

        void CheckBufferNo(List<int> li)
        {
            li.Sort();
            for (int i = 0; i < li.Count -1; i++)
            {
                if (li[i] > 63 || li[i] < 0)
                {
                    throw new Exception("Buffer号超出上下界");
                }
                if (li[i] == li[i + 1])
                {
                    throw new Exception("Buffer存在重复");
                }
            }
        }

        bool CheckMainController(CompInfoTemp c)
        {
            string t = c.content[KeyWordDef.AT];
            return GetCompFromType(t).IsMainController;
        }

        CompTemplate GetCompFromType(string type)
        {
            CompTemplate c = compTeltList.Find(s => s.Type == type);
            return c;
        }

        static string ContraScript(int bufferNo, Dictionary<ST, string> STContent, ST st)
        {
            return GetBufferSearchString(bufferNo) + STContent[st] + RNReturnStr;
        }

        static string GetBufferSearchString(int bufferNo)
        {
            string s = bufferNo.ToString();
            if (999 == bufferNo) { s = "A"; }
            return $"{RNReturnStr}#{s}{RNReturnStr}{bufferSearchStr}{RNReturnStr}";
        }

        public static int GetBufferIndex(int bufferNo, string scripts)
        {
            return scripts.IndexOf(GetBufferSearchString(bufferNo));
        }

        public static int GetBufferIndex(ST st, string scripts)
        {
            return GetBufferIndex(scriptNo[st][0], scripts);
        }

        public static int GetNextBufferIndex(int bufferNo, string scripts)
        {
            var SList = from sl in scriptNo
                        from si in sl.Value
                        select si;

            int nextBufferNo = SList.ToList()[SList.ToList().FindIndex(kp => kp == bufferNo) + 1];
            return GetBufferIndex(nextBufferNo, scripts);
        }

        public static int GetBufferCount(int bufferNo, string scripts)
        {
            int count = GetNextBufferIndex(bufferNo, scripts) - GetBufferIndex(bufferNo, scripts);
            return count;
        }
    }

    static class KeyWordDef
    {
        public const string AO = "AO";
        public const string SO = "SO";
        public const string AT = "AT";
        public const string AN = "AN";
        public const string HM = "HM";
        public const string HS = "HS";
        public const string HP = "HP";
        public const string HF = "HF";
        public const string HG = "HG";
        public const string CN = "CN";
        public const string CS = "CS";
        public const string CT = "CT";
        public const string MU = "MU";
        public const string BH = "BH";
        public const string BC = "BC";
        public const string BL = "BL";
        public const string BA = "BA";
        public const string TOP = "TOP";
        public const string INAME = "INAME";
        public const string ONAME = "ONAME";
        public const string ISI = "ISI";
        public const string OSI = "OSI";
        public const string SKIP = "SKIP";
        public const string LL = "LL";
        public const string RL = "RL";
        public const string LM = "LM";
        public const string CND = "CND";
        public const string NAME = "NAME";
        public const string SZ = "SZ";
    }
}
