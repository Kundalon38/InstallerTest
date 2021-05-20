using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Silent_PreInstall.Extensions;
using Silent_PreInstall.Models;

namespace Silent_PreInstall
{
    class Program
    {
        static int Main(string[] args)
        {
            _doTrace = true;                // TODO set False for production use
            writeTrace("b4 call FindBootstrapper");
            var bootData = FindBootstrapper();
            if (bootData != null)
            {
                writeTrace(bootData.ToString());
                return _sts_SILENT_OK;
            }
            writeTrace("bootData is null");

            //while(true) Thread.Sleep(500);
            return _sts_SILENT_EXCP;
        }

        // TODO document the (expected) relationship b/t processes
        public static BootstrapperData FindBootstrapper()
        {
            var curProcess = Process.GetCurrentProcess();
            var bootStrapperProcess = curProcess.GetParent();
            if (bootStrapperProcess == null) return null;

            var cmdLine = bootStrapperProcess.GetCommandLine();
            if (cmdLine == null) return null;

            var bootData = new BootstrapperData(cmdLine);
            if (!bootData.IsSilent) return bootData;

            var exePath = bootStrapperProcess.GetExecutablePath(); // yields the entire path, including filename
            bootData.LoadFileVersionInfo(FileVersionInfo.GetVersionInfo(exePath));

            return bootData;


            //var allProcs = Process.GetProcesses(".");
            //foreach (var curProcess in allProcs)
            //{
            //    if (string.Compare(curProcess.ProcessName, "msiexec", StringComparison.CurrentCultureIgnoreCase) != 0) continue;
            //    writeTrace($"FindBootStrapper msiexec found, pid={curProcess.Id}");
            //    var cmdLine = curProcess.GetCommandLine();
            //    if (cmdLine == null) continue;
            //    writeTrace($"FindBootStrapper msiexec pid={curProcess.Id}, cmdLine={cmdLine}");
            //    var cmdLineUC = cmdLine.ToUpper();
            //    if (!cmdLineUC.Contains(@"/I ")) continue;  // looking for msiexec /I

            //    var parentProcess = curProcess.GetParent();
            //    if (parentProcess == null) continue;        // not there, but should be

            //    writeTrace($"FindBootStrapper parent msiexec pid={parentProcess.Id}");
            //    cmdLine = parentProcess.GetCommandLine();
            //    if (cmdLine == null) continue;

            //    writeTrace($"FindBootStrapper parent msiexec pid={parentProcess.Id}, cmdLine={cmdLine}");
            //    bootData = new BootstrapperData(cmdLine);
            //    if (!bootData.IsSilent) continue;

            //    writeTrace($"Found Bootstrapper {parentProcess.ProcessName}");

            //    var exePath = parentProcess.GetExecutablePath(); // yields the entire path, including filename
            //    writeTrace($"exePath = {exePath}");

            //    bootData.LoadFileVersionInfo(FileVersionInfo.GetVersionInfo(exePath));
            //    break;

            //}
            //return bootData;
        }

        private static void writeTrace(string msg)
        {
            if (!_doTrace) return;
            if (_sw == null)
            {
                const string shortFn = "SilentPreReq-PreInstall.txt";
                var deskTopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                _sw = new StreamWriter(Path.Combine(deskTopPath, shortFn));
            }
            _sw.WriteLine(msg);
            _sw.Flush();
        }

        private static StreamWriter _sw;
        private static bool _doTrace;

        #region RISA_SI_PREINSTALL_RESULT return values

        // see CustomActions_consts in the main RISA_CustomActionsLib
        // MSI coerces these into strings to fit into a the RISA_SI_PREINSTALL_RESULT property

        public const int _sts_SILENT_OK = 1;
        public const int _sts_SILENT_EXCP = 0;
        public const int _sts_SILENT_ERR_REMOVE_INSTALLED_PRODUCT = -1;

        #endregion
    }
}
