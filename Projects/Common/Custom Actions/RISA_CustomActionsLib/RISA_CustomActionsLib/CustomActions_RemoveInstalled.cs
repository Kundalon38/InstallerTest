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
            #region copy Session props to SessionDTO
            SessionDTO sessDTO;
            try
            {
                sessDTO = remInstalled_copyFromSession(session);
            }
            catch (Exception e)
            {
                sessLog(session, e.Message);
                return ActionResult.Failure;
            }
            #endregion

            var actionResult = removeInstalledProducts(sessDTO);

            #region copy SessionDTO props back to Session
            try
            {
                remInstalled_copyToSession(session, sessDTO);
            }
            catch (Exception e)
            {
                sessLog(session, e.Message);
                return ActionResult.Failure;
            }
            #endregion

            return ActionResult.Success;
        }

        public static ActionResult removeInstalledProducts(SessionDTO sessDTO)
        {
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
                    //

                    var normalizedTargetDir = sessDTO[_propMSI_TARGETDIR].EnsureTrailingBash();
                    foreach (var insProd in insProductList)
                    {
                        if (string.Equals(insProd.TargetDir.EnsureTrailingBash(), normalizedTargetDir,
                            StringComparison.CurrentCultureIgnoreCase)) tbRemoved.Add(insProd);
                    }

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
                actionResult = ActionResult.Failure;
            }
            finally
            {
                sessLog(sessDTO, msgText);
            }
            return actionResult;
        }

        #region RemoveInstalledProducts - Session / SessionDTO property copying
        private static SessionDTO remInstalled_copyFromSession(Session session)
        {
            var sessDTO = new SessionDTO(session.Log)
            {
                // props set by installer
                [_propMSI_ProductName] = session[_propMSI_ProductName],

                // props set by other CA, read here
                [_propRISA_INSTALLED_PRODUCTS] = session[_propRISA_INSTALLED_PRODUCTS],
                //
                // props created by installer
                [_propRISA_STATUS_CODE] = session[_propRISA_STATUS_CODE],
                [_propRISA_STATUS_TEXT] = session[_propRISA_STATUS_TEXT],
            };
            return sessDTO;
        }

        private static void remInstalled_copyToSession(Session session, SessionDTO sessDTO)
        {
            // don't overwrite items set by caller, only those set here
            //
            session[_propRISA_STATUS_CODE] = sessDTO[_propRISA_STATUS_CODE];
            session[_propRISA_STATUS_TEXT] = sessDTO[_propRISA_STATUS_TEXT];
        }

        #endregion

    }
}
