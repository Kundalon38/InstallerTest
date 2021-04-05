using System;
using System.IO;
using Microsoft.Deployment.WindowsInstaller;
using RISA_CustomActionsLib.Models;

namespace RISA_CustomActionsLib
{
    public partial class CustomActions_StopStartService
    {

        private static bool reportError(SessionDTO sessDTO, string loc, string statusCode, string statusText)
        {
            sessDTO[_propRISA_STATUS_CODE] = statusCode;
            sessDTO[_propRISA_STATUS_TEXT] = statusText;
            sessLog(sessDTO, loc, statusText);
            return false;
        }

        private static void sessLog(SessionDTO sessDTO, string loc, string msg)
        {
            sessDTO.Log($"{loc}: {msg}");
            Trace(loc,msg);
        }

        private static void excpLog(Session session, string loc, Exception ex)
        {
            // typically this is called in an excp handler, on a failure to instantiate sessDTO
            var msg = $"{ex.Message}{Environment.NewLine}{ex.StackTrace}";
            session.Log($"{loc}: {msg}");
            Trace(loc, msg);
        }


        #region Trace

        public static void Trace(string loc, string msg)
        {
            if (!_doTrace) return;
            if (_sw == null)
            {
                try
                {
                    const string traceFileName = "CustomAction_trace.txt";
                    var desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    //
                    // open append; despite static methods, this class lib is re-instantiated for each Custom Action method
                    // - likewise, each CustomAction method wishing to Trace() must set _doTrace - earlier instances are lost
                    //
                    _sw = new StreamWriter(Path.Combine(desktopFolder, traceFileName),true);
                }
                catch (Exception)
                {
                }
            }
            if (_sw == null) return;
            try
            {
                const string dtFmt = "dd-MMM-yy HH:mm:ss";
                _sw.WriteLine($"{DateTime.Now.ToString(dtFmt)} {loc} {msg}");
                _sw.Flush();    // file is never explicitly Close()d, often the best is written last
            }
            catch (Exception)
            {
            }
        }
        private static bool _doTrace;
        private static StreamWriter _sw;

        #endregion
    }
}
