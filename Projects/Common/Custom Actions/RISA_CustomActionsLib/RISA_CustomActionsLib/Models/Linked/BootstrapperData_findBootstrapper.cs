using System;
using System.Diagnostics;

namespace RISA_CustomActionsLib.Models.Linked
{
    public partial class BootstrapperData
    {
        public static BootstrapperData FindBootstrapperFromCA(BootstrapperTestData testData = null)
        {
            var bootData = new BootstrapperData(testData);   // btd injects test data
            if (testData != null) return bootData;           // production use continues below

            var allProcs = Process.GetProcesses(".");
            foreach (var curProcess in allProcs)
            {
                if (!curProcess.ProcessName.IsEqIgnoreCase("msiexec")) continue;
                var cmdLine = curProcess.GetCommandLine();
                if (cmdLine == null) continue;
                var cmdLineUC = cmdLine.ToUpper();
                if (!cmdLineUC.Contains(@"/I ")) continue;  // looking for msiexec /I

                var bootStrapperProcess = curProcess.GetParent();
                if (bootStrapperProcess == null) continue;        // not there, but should be

                cmdLine = bootStrapperProcess.GetCommandLine();
                if (cmdLine == null) continue;

                // found it
                bootData = new BootstrapperData(cmdLine);
                if (bootData.IsSilent) bootData.GetExeData(bootStrapperProcess);
                break;
            }
            return bootData;
        }

        public static BootstrapperData FindBootstrapperFromExe(BootstrapperTestData testData = null)
        {
            var bootData = new BootstrapperData(testData);   // btd injects test data
            if (testData != null) return bootData;           // production use continues below

            var curProcess = Process.GetCurrentProcess();
            var bootStrapperProcess = curProcess.GetParent();
            if (bootStrapperProcess == null) return bootData;

            var cmdLine = bootStrapperProcess.GetCommandLine();
            if (cmdLine == null) return bootData;

            // found it
            bootData = new BootstrapperData(cmdLine);
            if (bootData.IsSilent) bootData.GetExeData(bootStrapperProcess);
            return bootData;
        }
    }
}
