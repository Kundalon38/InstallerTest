using System;
using System.Linq;
using Microsoft.Deployment.WindowsInstaller;
using RISA_CustomActionsLib.Models;
using RISA_CustomActionsLib.Models.Linked;
using BootstrapperData = RISA_CustomActionsLib.Models.Linked.BootstrapperData;

namespace RISA_CustomActionsLib
{
    public partial class CustomActions
    {
        [CustomAction]
        public static ActionResult SilentInitProperties(Session session)
        {
            const string methodName = "SilentInitProperties";

            #region copy Session props to SessionDTO
            SessionDTO sessDTO;
            try
            {
                sessDTO = silentInitProperties_getFromSession(session);
            }
            catch (Exception ex)
            {
                excpLog(session, methodName, ex);
                return ActionResult.Failure;
            }
            #endregion


            var retSts = silentInitProperties(sessDTO);

            #region copy SessionDTO props back to Session
            try
            {
                silentInitProperties_returnToSession(session, sessDTO);
            }
            catch (Exception ex)
            {
                excpLog(session, methodName, ex);
                return ActionResult.Failure;
            }
            #endregion

            return retSts.Result;
        }

        #region initProperties - without Session wrapper, for unit testing

        public static SilentResult silentInitProperties(SessionDTO sessDTO, BootstrapperTestData testData = null, ISiLog logger = null)
        {
            const string methodName = "silentInitProperties";
            ISiLog log = null;
            BootstrapperData bootData = null;
            try
            {
                bootData = BootstrapperData.FindBootstrapperFromCA(testData);
                if (bootData == null) return SilentResult.OK(bootData);
                if (!bootData.IsSilent) return SilentResult.OK(bootData);

                var validParse = bootData.ParseCmdLine();
                if (logger == null) log = new SiLog(bootData.LogFileName, false);
                else log = logger;

                foreach (var err in bootData.ErrorList) log.Write(methodName, err.Text);
                if (!validParse || bootData.ErrorList.Any(x => x.IsFatal)) return SilentResult.Fail(bootData);

                if (sessDTO[_propRISA_SI_PREINSTALL_RESULT] != _sts_SILENT_OK)
                {
                    log.Write(methodName, $"Aborting installation due to bad status" + 
                                          $" ({sessDTO[_propRISA_SI_PREINSTALL_RESULT]}) posted by Silent-PreInstall pre-req");
                    return SilentResult.Fail(bootData);
                }

                // assign properties - clone into InitProperties
                processProductVersion(sessDTO);
                assignVersionBasedProperties(sessDTO);
                assignRemainingIdentityBasedProperties(sessDTO);

                // assign install + documents directories
                var siDIRprop = bootData.CmdLineProperties[BootstrapperDataCommon._propInsDir];
                if(siDIRprop == null) assignDocumentPath(sessDTO);
                else
                {
                    var userSpecifiedInsDir = siDIRprop.PropValue;
                    string appDir;
                    if (pathContainsOneDrive(userSpecifiedInsDir) || pathIsRoaming(userSpecifiedInsDir))
                        appDir = altInstallDir(sessDTO[_propRISA_INSTALL_TYPE]);
                    else
                    {
                        var pgmFilesDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                        appDir = userSpecifiedInsDir.StartsWith(pgmFilesDir, StringComparison.CurrentCultureIgnoreCase)
                            ? altInstallDir(sessDTO[_propRISA_INSTALL_TYPE])
                            : userSpecifiedInsDir;
                    }
                    sessDTO[_propAI_APPDIR] = appDir;
                    sessDTO[_propRISA_USERFILES] = appDir;
                }
                log.Write(methodName, $"Installation directory resolved to: {sessDTO[_propAI_APPDIR]}");
                log.Write(methodName, $"Documents directory resolved to: {sessDTO[_propRISA_USERFILES]}");


                // assign license Type
                var siLTYprop = bootData.CmdLineProperties[BootstrapperDataCommon._propLicType];
                if (siLTYprop == null) assignDefaultLicenseType(sessDTO);
                else sessDTO[_propRISA_LICENSE_TYPE] = siLTYprop.PropValue;
                log.Write(methodName, $"License Type resolved to: {sessDTO[_propRISA_LICENSE_TYPE]}");

                // assign Region name, translate number (index) to country name string
                var siRGNprop = bootData.CmdLineProperties[BootstrapperDataCommon._propRegion];
                sessDTO[_propRISA_REGION_NAME] = siRGNprop == null
                    ? _defRegionName
                    : _regionNameList[int.Parse(siRGNprop.PropValue)];
                log.Write(methodName, $"Region Name resolved to: {sessDTO[_propRISA_REGION_NAME]}");

                // assign AutoUpdate, translate Yes / No to True / False
                var siUPDprop = bootData.CmdLineProperties[BootstrapperDataCommon._propUpdate];
                var finalValue = _boolTrue;
                if (siUPDprop != null)
                {
                    if (siUPDprop.PropValue == BootstrapperDataCommon._ansNo) finalValue = _boolFalse;
                }
                sessDTO[_propRISA_CHECK_UPDATES] = finalValue;
                log.Write(methodName, $"Auto Check for Updates resolved to: {sessDTO[_propRISA_CHECK_UPDATES]}");
                return SilentResult.OK(bootData);
            }
            catch (Exception e)
            {
                log?.Write(methodName, $"{e.Message}{Environment.NewLine}{e.StackTrace}");
                return SilentResult.Fail(bootData);
            }
        }

        #endregion


        #region Silent InitProperties - Session / SessionDTO property copying

        private static SessionDTO silentInitProperties_getFromSession(Session session)
        {
            var sessDTO = new SessionDTO(session.Log);
            // prop values set in aip or upstream
            copySinglePropFromSession(sessDTO, session, _propAI_APPDIR);
            copySinglePropFromSession(sessDTO, session, _propMSI_ProductName);
            copySinglePropFromSession(sessDTO, session, _propMSI_ProductVersion);
            copySinglePropFromSession(sessDTO, session, _propRISA_COMPANY_KEY);
            copySinglePropFromSession(sessDTO, session, _propRISA_INSTALL_TYPE);
            copySinglePropFromSession(sessDTO, session, _propRISA_REGISTRY_PRODUCT_NAME);
            //
            copySinglePropFromSession(sessDTO, session, _propRISA_SI_PREINSTALL_RESULT);
            //
            copySinglePropFromSession(sessDTO, session, _propRISA_CA_DEBUG, false);
            setupDebugIfRequested(session, sessDTO);
            //
            // prop values set by InitProperties
            copySinglePropFromSession(sessDTO, session, _propRISA_CHECK_UPDATES);
            copySinglePropFromSession(sessDTO, session, _propRISA_INSTALLED_PRODUCTS);
            copySinglePropFromSession(sessDTO, session, _propRISA_IS_ROAMING_PROFILE);
            copySinglePropFromSession(sessDTO, session, _propRISA_LICENSE_TYPE);
            copySinglePropFromSession(sessDTO, session, _propRISA_PRODUCT_TITLE2_INSTYPE);
            copySinglePropFromSession(sessDTO, session, _propRISA_PRODUCT_VERSION2);
            copySinglePropFromSession(sessDTO, session, _propRISA_PRODUCT_VERSION34);
            copySinglePropFromSession(sessDTO, session, _propRISA_REGION_NAME);
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
            session[_propRISA_CHECK_UPDATES] = sessDTO[_propRISA_CHECK_UPDATES];
            session[_propRISA_INSTALLED_PRODUCTS] = sessDTO[_propRISA_INSTALLED_PRODUCTS];
            session[_propRISA_IS_ROAMING_PROFILE] = sessDTO[_propRISA_IS_ROAMING_PROFILE];
            session[_propRISA_LICENSE_TYPE] = sessDTO[_propRISA_LICENSE_TYPE];
            session[_propRISA_PRODUCT_TITLE2_INSTYPE] = sessDTO[_propRISA_PRODUCT_TITLE2_INSTYPE];
            session[_propRISA_PRODUCT_VERSION2] = sessDTO[_propRISA_PRODUCT_VERSION2];
            session[_propRISA_PRODUCT_VERSION34] = sessDTO[_propRISA_PRODUCT_VERSION34];
            session[_propRISA_REGION_NAME] = sessDTO[_propRISA_REGION_NAME];
            session[_propRISA_STATUS_CODE] = sessDTO[_propRISA_STATUS_CODE];
            session[_propRISA_STATUS_TEXT] = sessDTO[_propRISA_STATUS_TEXT];
            session[_propRISA_UPDATE_DATA_VALUE] = sessDTO[_propRISA_UPDATE_DATA_VALUE];
            session[_propRISA_USERFILES] = sessDTO[_propRISA_USERFILES];
            //
            const string methodName = "initProperties_returnToSession";
            Trace(methodName, sessDTO.ToString());
        }

        #endregion

    }

}
