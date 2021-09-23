using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeePassLib.Interfaces;

namespace Skyhoshi.Configuration.Logging
{
    class DebugStatusLogger : IStatusLogger
    {
        public void StartLogging(string strOperation, bool bWriteOperationToLog)
        {
            CreateFirstEntry();
        }

        private void CreateFirstEntry()
        {
            System.Diagnostics.Debug.WriteLine($"STARTING Data Log");
        }

        public void EndLogging()
        {
            CreateLastEntry();
        }

        private void CreateLastEntry()
        {
            System.Diagnostics.Debug.WriteLine($"CLOSING Data Log");
        }

        public bool SetProgress(uint uPercent) { return true; }

        public bool SetText(string strNewText, LogStatusType lsType)
        {
            switch (lsType)
            {
                case LogStatusType.Info:
                    System.Diagnostics.Debug.WriteLine(strNewText);
                    break;
                case LogStatusType.Warning:
                    System.Diagnostics.Debug.WriteLine($"WARNING: {strNewText}");
                    break;
                case LogStatusType.Error:
                    System.Diagnostics.Debug.WriteLine($"ERROR: {strNewText}");
                    break;
                case LogStatusType.AdditionalInfo:
                    System.Diagnostics.Debug.WriteLine($"Additional Info: {strNewText}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lsType), lsType, null);
            }
            
            return true;
        }
        public bool ContinueWork() { return true; }

	}
}
