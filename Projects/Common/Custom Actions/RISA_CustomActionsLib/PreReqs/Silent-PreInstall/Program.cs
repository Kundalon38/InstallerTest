using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RISA_CustomActionsLib;
using RISA_CustomActionsLib.Models;
using RISA_CustomActionsLib.Models.Linked;
using Silent_PreInstall.Extensions;

namespace Silent_PreInstall
{
    class Program
    {
        static int Main(string[] args)
        {
            _doTrace = true;                // TODO set False for production use
            writeTrace("b4 call FindBootstrapper");
            var bootData = BootstrapperData.FindBootstrapperFromExe();




            return int.Parse(CustomActions._sts_SILENT_EXCP);        // TEST
            if (bootData != null)
            {
                writeTrace(bootData.ToString());
                return int.Parse(CustomActions._sts_SILENT_OK);
            }
            writeTrace("bootData is null");

            //while(true) Thread.Sleep(500);
            return int.Parse(CustomActions._sts_SILENT_EXCP);
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

    }
}
