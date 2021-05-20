using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RISA_CustomActionsLib.Extensions;
using RISA_CustomActionsLib.Models;

namespace RISA_CustomActionsLib
{
    public partial class CustomActions
    {
        // TODO document the (expected) relationship b/t processes
        public static BootstrapperData FindBootstrapper(string callerMethodName)
        {
            BootstrapperData bootData = null;

            var allProcs = Process.GetProcesses(".");
            foreach (var curProcess in allProcs)
            {
                if (string.Compare(curProcess.ProcessName, "msiexec", StringComparison.CurrentCultureIgnoreCase) != 0) continue;
                var cmdLine = curProcess.GetCommandLine();
                if (cmdLine == null) continue;
                var cmdLineUC = cmdLine.ToUpper();
                if (!cmdLineUC.Contains(@"/I ")) continue;  // looking for msiexec /I

                var parentProcess = curProcess.GetParent();
                if (parentProcess == null) continue;        // not there, but should be

                cmdLine = parentProcess.GetCommandLine();
                if (cmdLine == null) continue;

                bootData = new BootstrapperData(cmdLine);
                if (!bootData.IsSilent) continue;

                Trace(callerMethodName, $"Found Bootstrapper {parentProcess.ProcessName}");

                var exePath = parentProcess.GetExecutablePath(); // yields the entire path, including filename
                Trace(callerMethodName, $"exePath = {exePath}");

                bootData.LoadFileVersionInfo(FileVersionInfo.GetVersionInfo(exePath));
                break;

            }
            return bootData;
        }
    }
}
