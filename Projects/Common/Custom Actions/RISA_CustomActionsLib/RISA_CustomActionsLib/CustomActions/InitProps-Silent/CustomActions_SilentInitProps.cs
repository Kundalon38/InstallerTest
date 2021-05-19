using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.WindowsInstaller;
using RISA_CustomActionsLib.Extensions;
using RISA_CustomActionsLib.Models;

namespace RISA_CustomActionsLib
{
    public partial class CustomActions
    {
        [CustomAction]
        public static ActionResult SilentInitProperties(Session session)
        {
            const string methodName = "SilentInitProperties";

            _doTrace = true;        // TODO debug

            #region copy Session props to SessionDTO
            SessionDTO sessDTO;
            try
            {
                sessDTO = silentInitProperties_getFromSession(session);
            }
            catch (Exception ex)
            {
                excpLog(session, methodName, ex);
                return ActionResult.Success;
            }
            #endregion

            Trace(methodName,$"{_propRISA_SILENT_PREINSTALL_STATUS}={sessDTO[_propRISA_SILENT_PREINSTALL_STATUS]}");
            var bootData = FindBootstrapper();
            Trace(methodName, bootData.ToString());


            silentInitProperties(sessDTO);  // TODO

            #region copy SessionDTO props back to Session
            try
            {
                silentInitProperties_returnToSession(session, sessDTO);
            }
            catch (Exception ex)
            {
                excpLog(session, methodName, ex);
                return ActionResult.Success;
            }
            #endregion

            return ActionResult.Success;
        }

        // TODO document the (expected) relationship b/t processes
        public static BootstrapperData FindBootstrapper()
        {
            const string methodName = "SilentInitProperties";
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

                Trace(methodName, $"Found Bootstrapper {parentProcess.ProcessName}");

                var exePath = parentProcess.GetExecutablePath(); // yields the entire path, including filename
                Trace(methodName, $"exePath = {exePath}");

                bootData.LoadFileVersionInfo(FileVersionInfo.GetVersionInfo(exePath));
                break;

            }
            return bootData;
        }



        #region Silent InitProperties - Session / SessionDTO property copying

        private static SessionDTO silentInitProperties_getFromSession(Session session)
        {
            var sessDTO = new SessionDTO(session.Log);
            // prop values set in aip
            copySinglePropFromSession(sessDTO, session, _propAI_APPDIR);
            copySinglePropFromSession(sessDTO, session, _propMSI_ProductName);
            copySinglePropFromSession(sessDTO, session, _propMSI_ProductVersion);
            copySinglePropFromSession(sessDTO, session, _propRISA_COMPANY_KEY);
            copySinglePropFromSession(sessDTO, session, _propRISA_INSTALL_TYPE);
            copySinglePropFromSession(sessDTO, session, _propRISA_REGISTRY_PRODUCT_NAME);
            //
            copySinglePropFromSession(sessDTO, session, _propRISA_SILENT_PREINSTALL_STATUS);
            //
            copySinglePropFromSession(sessDTO, session, _propRISA_CA_DEBUG, false);
            setupDebugIfRequested(session, sessDTO);
            //
            // prop values set by InitProperties
            copySinglePropFromSession(sessDTO, session, _propRISA_INSTALLED_PRODUCTS);
            copySinglePropFromSession(sessDTO, session, _propRISA_IS_ROAMING_PROFILE);
            copySinglePropFromSession(sessDTO, session, _propRISA_LICENSE_TYPE);
            copySinglePropFromSession(sessDTO, session, _propRISA_PRODUCT_TITLE2_INSTYPE);
            copySinglePropFromSession(sessDTO, session, _propRISA_PRODUCT_VERSION2);
            copySinglePropFromSession(sessDTO, session, _propRISA_PRODUCT_VERSION34);
            copySinglePropFromSession(sessDTO, session, _propRISA_STATUS_CODE);
            copySinglePropFromSession(sessDTO, session, _propRISA_STATUS_TEXT);
            copySinglePropFromSession(sessDTO, session, _propRISA_UPDATE_DATA_VALUE);
            copySinglePropFromSession(sessDTO, session, _propRISA_USERFILES);
            return sessDTO;
        }

        private static void silentInitProperties_returnToSession(Session session, SessionDTO sessDTO)
        {
            // don't overwrite items set by caller, only those set here
            //
            session[_propAI_APPDIR] = sessDTO[_propAI_APPDIR];
            session[_propRISA_INSTALLED_PRODUCTS] = sessDTO[_propRISA_INSTALLED_PRODUCTS];
            session[_propRISA_IS_ROAMING_PROFILE] = sessDTO[_propRISA_IS_ROAMING_PROFILE];
            session[_propRISA_LICENSE_TYPE] = sessDTO[_propRISA_LICENSE_TYPE];
            session[_propRISA_PRODUCT_TITLE2_INSTYPE] = sessDTO[_propRISA_PRODUCT_TITLE2_INSTYPE];
            session[_propRISA_PRODUCT_VERSION2] = sessDTO[_propRISA_PRODUCT_VERSION2];
            session[_propRISA_PRODUCT_VERSION34] = sessDTO[_propRISA_PRODUCT_VERSION34];
            session[_propRISA_STATUS_CODE] = sessDTO[_propRISA_STATUS_CODE];
            session[_propRISA_STATUS_TEXT] = sessDTO[_propRISA_STATUS_TEXT];
            session[_propRISA_UPDATE_DATA_VALUE] = sessDTO[_propRISA_UPDATE_DATA_VALUE];
            session[_propRISA_USERFILES] = sessDTO[_propRISA_USERFILES];
            //
            const string methodName = "initProperties_returnToSession";
            Trace(methodName, sessDTO.ToString());
        }

        #endregion

        #region initProperties - without Session wrapper, for unit testing

        public static void silentInitProperties(SessionDTO sessDTO)
        {
            const string methodName = "silentInitProperties";
            string msgText = null;
            try
            {
                //if (!validProductName(sessDTO)) return;
                //if (!processProductVersion(sessDTO)) return;
                //if (!validInstallType(sessDTO)) return;
                //if (!noRISAproductsAreRunning(sessDTO)) return;

                assignVersionBasedProperties(sessDTO);
                //if (!serializeMatchingInstalledProducts(sessDTO)) return;
                assignRemainingIdentityBasedProperties(sessDTO);
                assignDocumentPath(sessDTO);
                assignDefaultLicenseType(sessDTO);

                sessDTO[_propRISA_STATUS_CODE] = _sts_OK;
                sessDTO[_propRISA_STATUS_TEXT] = _stsText_Success; ;
            }
            catch (Exception e)
            {
                sessDTO[_propRISA_STATUS_CODE] = _sts_EXCP;
                msgText = $"{e.Message}{Environment.NewLine}{e.StackTrace}";
            }
            finally
            {
                sessLog(sessDTO, methodName, msgText);
            }
        }

        #endregion
    }

}
