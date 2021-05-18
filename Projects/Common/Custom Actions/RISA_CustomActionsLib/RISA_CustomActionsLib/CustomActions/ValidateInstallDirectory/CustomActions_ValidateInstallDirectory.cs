using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.WindowsInstaller;
using RISA_CustomActionsLib.Models;

namespace RISA_CustomActionsLib
{
    public partial class CustomActions
    {
        [CustomAction]
        public static ActionResult ValidateInstallDirectory(Session session)
        {
            const string methodName = "ValidateInstallDirectory";
            #region copy Session props to SessionDTO
            SessionDTO sessDTO;
            try
            {
                sessDTO = valInstallDir_getFromSession(session);
            }
            catch (Exception ex)
            {
                excpLog(session, methodName, ex);
                return ActionResult.Success;
            }
            #endregion

            validateInstallDirectory(sessDTO);

            #region copy SessionDTO props back to Session
            try
            {
                valInstallDir_returnToSession(session, sessDTO);
            }
            catch (Exception ex)
            {
                excpLog(session, methodName, ex);
                return ActionResult.Success;
            }
            #endregion

            return ActionResult.Success;
        }

        public static void validateInstallDirectory(SessionDTO sessDTO, bool? isRoamingValue = null)
        {
            const string methodName = "removeInstalledProducts";
            const string badDirMsg = "Destination Folder Not Allowed";
            string msgText = null;
            try
            {
                var doValidateInsDir = isAproblemProfile(isRoamingValue);
                if (doValidateInsDir)
                {
                    var appDir = sessDTO[_propAI_APPDIR].Trim();
                    var pgmFilesDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                    if (appDir.StartsWith(pgmFilesDir, StringComparison.CurrentCultureIgnoreCase))
                    {
                        sessDTO[_propAI_APPDIR] = defaultRoamingInstallDir(sessDTO[_propRISA_INSTALL_TYPE]);
                        sessDTO[_propRISA_STATUS_CODE] = _sts_BAD_DEST_DIR;
                        msgText = badDirMsg;
                        return;
                    }
                }
                sessDTO[_propRISA_STATUS_CODE] = _sts_OK;
                msgText = _stsText_Success;
            }
            catch (Exception e)
            {
                sessDTO[_propRISA_STATUS_CODE] = _sts_EXCP;
                msgText = $"{e.Message}{Environment.NewLine}{e.StackTrace}";
            }
            finally
            {
                sessDTO[_propRISA_STATUS_TEXT] = msgText;
                sessLog(sessDTO, methodName, msgText);
            }
        }

        #region ValidateInstallDirectory - Session / SessionDTO property copying

        private static SessionDTO valInstallDir_getFromSession(Session session)
        {
            var sessDTO = new SessionDTO(session.Log);
            // prop values set in aip
            copySinglePropFromSession(sessDTO, session, _propAI_APPDIR);
            //
            copySinglePropFromSession(sessDTO, session, _propRISA_CA_DEBUG, false);
            setupDebugIfRequested(session, sessDTO);
            //
            copySinglePropFromSession(sessDTO, session, _propRISA_INSTALL_TYPE);
            //
            // prop values set by ValidateInstallDirectory
            copySinglePropFromSession(sessDTO, session, _propRISA_STATUS_CODE);
            copySinglePropFromSession(sessDTO, session, _propRISA_STATUS_TEXT);
            //
            const string methodName = "valInstallDir_getFromSession";
            Trace(methodName, sessDTO.ToString());
            return sessDTO;
        }

        private static void valInstallDir_returnToSession(Session session, SessionDTO sessDTO)
        {
            // don't overwrite items set by caller, only those set here
            //
            session[_propRISA_STATUS_CODE] = sessDTO[_propRISA_STATUS_CODE];
            session[_propRISA_STATUS_TEXT] = sessDTO[_propRISA_STATUS_TEXT];
        }

        #endregion
    }
}
