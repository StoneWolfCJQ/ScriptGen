using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptGen
{
    class CompTemplate
    {
        public virtual string Type { get; }
        protected virtual string ACSStr { get { return "ACS"; } }
        public virtual bool IsMainController { get { throw new NotImplementedException(); } }
        public virtual void FillSlaveAndACSAxis(ref int slaveIndex, ref int axisIndex, 
            Dictionary<string, string> source, ref CompInfoTemp output)
        {
            int ao = GetAO(source, output);
            if (Type != ACSStr)
            {
                ao = 0;
            }
            output.axisStart = axisIndex;
            output.slaveStart = slaveIndex;
            output.axisOccupied = ao;
            output.slaveOccupied = GetSO(source, output);
            axisIndex += output.axisOccupied;
            slaveIndex += output.slaveOccupied;
        }

        public virtual void FillAllAxisAndContent(ref int axisIndex, 
            Dictionary<string, string> source, 
            ref CompInfoTemp output)
        {
            FillContent(source, ref output);
            if (Type != ACSStr)
            {
                output.axisStart = axisIndex;
                output.axisOccupied = GetAO(source, output);
                axisIndex += output.axisOccupied;
            }
        }

        protected virtual void FillContent(Dictionary<string, string> source, ref CompInfoTemp output)
        {
            foreach (var kv in source)
            {
                DictionaryFunctions.AddOnlyNewPair(kv, output.content);
            }
        }

        public virtual void WriteScript(CompInfoTemp c, Dictionary<ST, List<int>>scriptNo, ref string scripts)
        {
            if (IsSkip(c)) return;
            WriteHome(c, scriptNo[ST.HOME], ref scripts);
            WriteComp(c, scriptNo[ST.COMP][0], scriptNo[ST.HOME], ref scripts);
            WriteLaser(c, scriptNo[ST.LASER][0], ref scripts);
            WriteAuto(c, scriptNo[ST.AUTO][0], ref scripts);
            WriteDBuffer(c, ref scripts);
        }

        public virtual void FillContentFromDefLine(CompInfoTemp t, string defLine)
        {
            Dictionary<string, string> dt = RegFunctions.GetDefLineDict(defLine);
            foreach (var kv in dt)
            {
                DictionaryFunctions.AddPair(kv, t.content);
            }
        }

        protected virtual bool IsSkip(CompInfoTemp c)
        {
            return c.content.ContainsKey("SKIP");
        }

        protected virtual int GetAO(Dictionary<string, string> d, CompInfoTemp output)
        {
            return int.Parse(d[KeyWordDef.AO]);
        }

        protected virtual int GetSO(Dictionary<string, string> d, CompInfoTemp output)
        {
            return int.Parse(d[KeyWordDef.SO]);
        }

        protected virtual void WriteHome(CompInfoTemp c, List<int> homeBufferNo, ref string scripts)
        {
            throw new NotImplementedException();
        }

        protected virtual void WriteComp(CompInfoTemp c, int compBufferNo, List<int> homeBufferNo, ref string scripts)
        {
            throw new NotImplementedException();
        }

        protected virtual void WriteLaser(CompInfoTemp c, int laserBufferNo, ref string scripts)
        {
            throw new NotImplementedException();
        }

        protected virtual void WriteAuto(CompInfoTemp c, int autoBufferNo, ref string scripts)
        {
            throw new NotImplementedException();
        }

        protected virtual void WriteDBuffer(CompInfoTemp c, ref string scripts)
        {
            throw new NotImplementedException();
        }
    }
}
