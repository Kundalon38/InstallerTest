using System;
using System.Collections.Generic;
using Microsoft.Deployment.WindowsInstaller;
using RISA_CustomActionsLib.Extensions;
using RISA_CustomActionsLib.Models;

namespace RISA_CustomActionsLib
{
    public partial class CustomActions
    {
        [CustomAction]
        public static ActionResult RemoveInstalledProducts(Session session)
        {
            const string methodName = "RemoveInstalledProducts";
            #region copy Session props to SessionDTO
            SessionDTO sessDTO;
            try
            {
                sessDTO = remInstalled_getFromSession(session);
            }
            catch (Exception ex)
            {
                excpLog(session, methodName, ex);
                return ActionResult.Success;
            }
            #endregion

            var actionResult = removeInstalledProducts(sessDTO);

            #region copy SessionDTO props back to Session
            try
            {
                remInstalled_returnToSession(session, sessDTO);
            }
            catch (Exception ex)
            {
                excpLog(session, methodName, ex);
                return ActionResult.Success;
            }
            #endregion

            return ActionResult.Success;
        }

        public static ActionResult removeInstalledProducts(SessionDTO sessDTO)
        {
            const string methodName = "removeInstalledProducts";
            var actionResult = ActionResult.Success;
            string msgText = null;
            try
            {
                var insProdListXml = sessDTO[_propRISA_INSTALLED_PRODUCTS];
                if (!string.IsNullOrEmpty(insProdListXml))
                {
                    var tbRemoved = new List<InstalledProduct>();
                    var insProductList = InstalledProductList.Deserialize(insProdListXml);
                    //
                    // normalize the two directory strings, sometimes there's a trailing bash, sometimes not
                    // - AI install directory is APPDIR (result of their dialog), not TARGETDIR
                    //
                    var normalizedAppDir = sessDTO[_propAI_APPDIR].EnsureTrailingBash();
                    Trace(methodName, $"normalizedAppDir={normalizedAppDir}");
                    foreach (var insProd in insProductList)
                    {
                        if (string.Equals(insProd.TargetDir.EnsureTrailingBash(), normalizedAppDir,
                            StringComparison.CurrentCultureIgnoreCase)) tbRemoved.Add(insProd);
                    }
                    Trace(methodName, $"tbRemoved.Count={tbRemoved.Count}");
                    foreach (var insProd in tbRemoved) insProd.UnInstall();
                }
                sessDTO[_propRISA_STATUS_CODE] = _sts_OK;
                msgText = "Success";
                sessDTO[_propRISA_STATUS_TEXT] = msgText;
            }
            catch (Exception e)
            {
                sessDTO[_propRISA_STATUS_CODE] = _sts_EXCP;
                msgText = $"{e.Message}{Environment.NewLine}{e.StackTrace}";
                actionResult = ActionResult.Success;
            }
            finally
            {
                sessLog(sessDTO, methodName, msgText);
            }
            return actionResult;
        }

        #region RemoveInstalledProducts - Session / SessionDTO property copying

        private static SessionDTO remInstalled_getFromSession(Session session)
        {
            var sessDTO = new SessionDTO(session.Log);
            // prop values set in aip
            copySinglePropFromSession(sessDTO, session, _propMSI_ProductName);
            copySinglePropFromSession(sessDTO, session, _propAI_APPDIR);
            //
            copySinglePropFromSession(sessDTO, session, _propRISA_CA_DEBUG, false);
            setupDebugIfRequested(session, sessDTO);
            //
            //
            // prop values set by antecedent call to InitProperties
            copySinglePropFromSession(sessDTO, session, _propRISA_INSTALLED_PRODUCTS);
            //
            // prop values set by RemoveInstalledProducts
            copySinglePropFromSession(sessDTO, session, _propRISA_STATUS_CODE);
            copySinglePropFromSession(sessDTO, session, _propRISA_STATUS_TEXT);
            //
            const string methodName = "remInstalled_getFromSession";
            Trace(methodName, sessDTO.ToString());
            return sessDTO;
        }

        private static void remInstalled_returnToSession(Session session, SessionDTO sessDTO)
        {
            // don't overwrite items set by caller, only those set here
            //
            session[_propRISA_STATUS_CODE] = sessDTO[_propRISA_STATUS_CODE];
            session[_propRISA_STATUS_TEXT] = sessDTO[_propRISA_STATUS_TEXT];
        }

        #endregion

    }
}
