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
        };

        static Dictionary<ST, List<int>> scriptNo;
        static Dictionary<int, int> scriptIndex;

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

        public string GenerateScript(Dictionary<ST, string> STContent, List<CompInfoTemp> topoList)
        {
            if (!CheckMainController(topoList[0]))
            {
                throw new Exception("第一个部件不是EC");
            }
            string baseScripts = GenerateBase(STContent, topoList[0]);
            for (int i = 1; i < topoList.Count; i++) 
            {
                CompInfoTemp c = topoList[i];
                if (CheckMainController(c))
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
            scriptIndex = new Dictionary<int, int>();
            List<int> homeBuffer = c.content[KeyWordDef.BH].Split(",".ToArray(),StringSplitOptions.RemoveEmptyEntries)
                .Select(s=>int.Parse(s)).ToList();
            int compBuffer = int.Parse(c.content[KeyWordDef.BC]);
            int laserBuffer = int.Parse(c.content[KeyWordDef.BL]);
            int autoBuffer = int.Parse(c.content[KeyWordDef.BA]);
            string ss = "";
            string rn = "\r\n";
            List<int> li = new List<int>();
            foreach (int i in homeBuffer)
            {
                scriptIndex.Add(i, ss.Length);
                ss += $"{rn}#{i}{rn}" + STContent[ST.HOME] + rn;
                li.Add(i);
            }
            scriptNo.Add(ST.HOME, li);

            scriptIndex.Add(compBuffer, ss.Length);
            ss += $"{rn}#{compBuffer}{rn}" + STContent[ST.COMP] + rn;
            scriptNo.Add(ST.COMP, new List<int>() { compBuffer });
            li.Add(compBuffer);

            scriptIndex.Add(laserBuffer, ss.Length);
            ss += $"{rn}#{laserBuffer}{rn}" + STContent[ST.LASER] + rn;
            scriptNo.Add(ST.LASER, new List<int>() { laserBuffer });
            li.Add(laserBuffer);

            scriptIndex.Add(autoBuffer, ss.Length);
            ss += $"{rn}#{autoBuffer}{rn}" + STContent[ST.AUTO] + rn;
            scriptNo.Add(ST.AUTO, new List<int>() { autoBuffer });
            li.Add(autoBuffer);

            CheckBufferNo(li);

            scriptIndex.Add(999, ss.Length);
            ss += $"{rn}#A{rn}" + STContent[ST.DEF];
            scriptNo.Add(ST.DEF, new List<int>() { 999 });
            return ss;
        }

        void CheckBufferNo(List<int> li)
        {
            li.Sort();
            for (int i = 0; i < li.Count -1; i++)
            {
                if (li[i] > 31 || li[i] < 0)
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

        public static int GetBufferIndex(int bufferNo)
        {
            return scriptIndex[bufferNo];
        }

        public static int GetBufferIndex(ST st)
        {
            return GetBufferIndex(scriptNo[st][0]);
        }

        public static List<int> GetBufferIndexes(ST st)
        {
            return scriptNo[st].Select(i => GetBufferIndex(i)).ToList();
        }

        public static int GetNextBufferIndex(int bufferNo)
        {
            int index = scriptIndex.ToList()
                [scriptIndex.ToList().FindIndex(kp => kp.Key == bufferNo) + 1]
                .Value;
            return index;
        }

        public static int GetBufferCount(int bufferNo)
        {
            int count = GetBufferIndex(bufferNo) - GetNextBufferIndex(bufferNo);
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
    }
}
