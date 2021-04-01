using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;
using RISA_CustomActionsLib.Models;


namespace RISA_CustomActionsLib
{
    public partial class CustomActions
    {
        // many minor methods are public for the sake of unit testing
        // - they really should be private
        // - these methods begin in lowerCase
        // - public methods called by MSI begin in UpperCase

        [CustomAction]
        public static ActionResult InitProperties(Session session)
        {
#if DEBUG
            int processId = Process.GetCurrentProcess().Id;
            string message = string.Format("Init Properties: Please attach the debugger (elevated) to process [{0}].", processId);
            MessageBox.Show(message, "Debug");
#endif

            #region copy Session props to SessionDTO
            SessionDTO sessDTO;
            try
            {
                sessDTO = initProperties_copyFromSession(session);
            }
            catch (Exception e)
            {
                sessLog(session, e.Message);
                return ActionResult.Failure;
            }
            #endregion

            var actionResult = initProperties(sessDTO);

            #region copy SessionDTO props back to Session
            try
            {
                initProperties_copyToSession(session, sessDTO);
            }
            catch (Exception e)
            {
                sessLog(session, e.Message);
                return ActionResult.Failure;
            }
            #endregion

            return actionResult;
        }

        #region InitProperties - Session / SessionDTO property copying
        private static SessionDTO initProperties_copyFromSession(Session session)
        {
            var sessDTO = new SessionDTO(session.Log)
            {
                // props set by installer
                [_propMSI_ProductName] = session[_propMSI_ProductName],
                [_propMSI_ProductVersion] = session[_propMSI_ProductVersion],
                [_propRISA_COMPANY_KEY] = session[_propRISA_COMPANY_KEY],
                [_propRISA_INSTALL_TYPE] = session[_propRISA_INSTALL_TYPE],
                [_propRISA_REGISTRY_PRODUCT_NAME] = session[_propRISA_REGISTRY_PRODUCT_NAME],

                // props set here
                [_propRISA_INSTALLED_PRODUCTS] = session[_propRISA_INSTALLED_PRODUCTS],
                [_propRISA_LICENSE_TYPE] = session[_propRISA_LICENSE_TYPE],
                [_propRISA_PRODUCT_TITLE2_INSTYPE] = session[_propRISA_PRODUCT_TITLE2_INSTYPE],
                [_propRISA_PRODUCT_VERSION2] = session[_propRISA_PRODUCT_VERSION2],
                [_propRISA_PRODUCT_VERSION34] = session[_propRISA_PRODUCT_VERSION34],
                [_propRISA_STATUS_CODE] = session[_propRISA_STATUS_CODE],
                [_propRISA_STATUS_TEXT] = session[_propRISA_STATUS_TEXT],
                [_propRISA_UPDATE_DATA_VALUE] = session[_propRISA_UPDATE_DATA_VALUE],
                [_propRISA_USERFILES] = session[_propRISA_USERFILES],
                [_propUSERFILES_RISA] = session[_propUSERFILES_RISA]    // deprecate
            };
            return sessDTO;
        }

        private static void initProperties_copyToSession(Session session, SessionDTO sessDTO)
        {
            // don't overwrite items set by caller, only those set here
            //
            session[_propRISA_INSTALLED_PRODUCTS] = sessDTO[_propRISA_INSTALLED_PRODUCTS];
            session[_propRISA_LICENSE_TYPE] = sessDTO[_propRISA_LICENSE_TYPE];
            session[_propRISA_PRODUCT_TITLE2_INSTYPE] = sessDTO[_propRISA_PRODUCT_TITLE2_INSTYPE];
            session[_propRISA_PRODUCT_VERSION2] = sessDTO[_propRISA_PRODUCT_VERSION2];
            session[_propRISA_PRODUCT_VERSION34] = sessDTO[_propRISA_PRODUCT_VERSION34];
            session[_propRISA_STATUS_CODE] = sessDTO[_propRISA_STATUS_CODE];
            session[_propRISA_STATUS_TEXT] = sessDTO[_propRISA_STATUS_TEXT];
            session[_propRISA_UPDATE_DATA_VALUE] = sessDTO[_propRISA_UPDATE_DATA_VALUE];
            session[_propRISA_USERFILES] = sessDTO[_propRISA_USERFILES];
            session[_propUSERFILES_RISA] = sessDTO[_propUSERFILES_RISA];    // deprecate
        }

        #endregion

        #region initProperties - without Session wrapper, for unit testing

        public static ActionResult initProperties(SessionDTO sessDTO)
        {
            var actionResult = ActionResult.Success;
            string msgText = null;
            try
            {
                if (!validProductName(sessDTO)) return ActionResult.Failure;
                if (!processProductVersion(sessDTO)) return ActionResult.Failure;
                if (!validInstallType(sessDTO)) return ActionResult.Failure;

                assignVersionBasedProperties(sessDTO);
                if(!serializeMatchingInstalledProducts(sessDTO)) return ActionResult.Failure;
                assignRemainingIdentityBasedProperties(sessDTO);
                assignDocumentPath(sessDTO);
                assignDefaultLicenseType(sessDTO);

                if (_versionParts.Length == 3)
                {
                    sessDTO[_propRISA_STATUS_CODE] = _sts_WARN_VERSION3;
                    msgText = "ProductVersion should have 4 version parts";
                }
                else
                {
                    sessDTO[_propRISA_STATUS_CODE] = _sts_OK;
                    msgText = "Success";
                }
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
                sessDTO[_propRISA_PROPS_ARE_INITIALIZED] = "True";
            }
            return actionResult;
        }


        #endregion

        #region InitProperties - assignment

        private static string[] _versionParts;  // set in processProductVersion()

        public static void assignVersionBasedProperties(SessionDTO session)
        {
            // versions are (MS terminology): major.minor.build.revision
            //
            // version2 = major.minor version parts
            var version2 = $"{_versionParts[0]}.{_versionParts[1]}";
            session[_propRISA_PRODUCT_VERSION2] = version2;

            // prodDisplayName eventually = eg RISA-3D 19.0  or  RISA-3D Demo 19.0
            var prodDisplayName = $"{session[_propMSI_ProductName]} {version2}";
            if (session[_propRISA_INSTALL_TYPE] == _insTypeDemo) prodDisplayName += $" {_insTypeDemo}";
            session[_propRISA_PRODUCT_TITLE2_INSTYPE] = prodDisplayName;

            // build.revision version parts; allow that revision may be omitted
            var vers4 = "0";
            if (_versionParts.Length == 4) vers4 = _versionParts[3];
            session[_propRISA_PRODUCT_VERSION34] = $"{_versionParts[2]}.{vers4}";
        }


        public static void assignRemainingIdentityBasedProperties(SessionDTO session)
        {
            var prodNdx = Array.IndexOf(_productNameList, session[_propMSI_ProductName]);
            var updateSfx = session[_propRISA_INSTALL_TYPE] == _insTypeDemo
                                    ? $"_{_insTypeDemo}" : "_SA";

            // this gets written to the registry, generate the value here, such as eg "UpdateData3D_SA"
            session[_propRISA_UPDATE_DATA_VALUE] = $"UpdateData{_updateProductAbbrevs[prodNdx]}{updateSfx}";

        }

        public static void assignDefaultLicenseType(SessionDTO session)
        {
            // try to find user's earlier choice in registry, if it exists
            //
            // Note the special, one time use of _propRISA_REGISTRY_PRODUCT_NAME instead of _propMSI_ProductName
            //  if this is not set, an excp occurs (caught in the empty catch block)
            //   and we return _defLicenseType, regardless of what may / may not be in registry

            var licenseTypeValue = _defLicenseType; // assign this if not in registry
            try
            {
                // code below only works if a previous version of product was installed, or another version is still installed
                //  license types are per-product, span versions, and live on after uninstall
                //
                var regKey = $@"SOFTWARE\{session[_propRISA_COMPANY_KEY]}\{session[_propRISA_REGISTRY_PRODUCT_NAME]}";
                using (var key = Registry.LocalMachine.OpenSubKey(regKey, RegistryKeyPermissionCheck.ReadSubTree))
                {
                    if (key != null)
                    {
                        var o = key.GetValue(_licenseTypeSubKeyName);
                        if (o != null) licenseTypeValue = o as string;
                    }
                }
            }
            catch (Exception)
            {
                // ignore
            }
            session[_propRISA_LICENSE_TYPE] = licenseTypeValue;
        }

        #endregion

        #region InitProperties - roaming profile

        public static void assignDocumentPath(SessionDTO session, bool? isRoamingValue = null)
        {
            // typical myDocsPath:      C:\Users\<username>\Documents
            var myDocsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var usingOneDrive = pathContainsOneDrive(myDocsPath);

            // typical userProfilePath: C:\Users\<username>
            var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (!usingOneDrive) usingOneDrive = pathContainsOneDrive(userProfilePath);

            // roaming if a unc path
            var isRoaming = isRoamingValue.HasValue ? isRoamingValue.Value : myDocsPath.StartsWith(@"\\");

            if (!isRoaming)
            {
                // myDocsPath must stem from userProfilePath, otherwise isRoaming=T; will this comparison always work?
                isRoaming = !myDocsPath.StartsWith(userProfilePath);
            }

            if (!isRoaming)
            {
                // require both paths to exist, otherwise isRoaming=T
                isRoaming = !(Directory.Exists(myDocsPath) && Directory.Exists(userProfilePath));
            }

            string installTypeProperty = session[_propRISA_INSTALL_TYPE];
            string folderName = (installTypeProperty == _insTypeDemo) ? "RISADemo" : "RISA";

            string docsPath;
            if (isRoaming || usingOneDrive)
            {
                // this is about finding the correct drive letter based on where the Windows directory lives
                // nearly everyone will be using C:\
                //
                const string bash = @"\";
                var drives = DriveInfo.GetDrives();
                var windowsDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                var outDriveIfRoaming = drives.Select(x => x.Name).SingleOrDefault(x => windowsDir.StartsWith(x));
                if (outDriveIfRoaming == null) outDriveIfRoaming = @"C:\";
                else if (!outDriveIfRoaming.EndsWith(bash)) outDriveIfRoaming += bash;
                var outputDirIfRoaming = $"{outDriveIfRoaming}{folderName}";
                docsPath = outputDirIfRoaming;
            }
            else docsPath = Path.Combine(myDocsPath, folderName);

            session[_propRISA_USERFILES] = docsPath;
            session[_propUSERFILES_RISA] = docsPath;           // deprecated
        }

        public static bool pathContainsOneDrive(string path)
        {
            // OneDrive is at:      C:\Users\<username>\OneDrive    users can add subdirs to this
            // but not completely sure of all use cases
            //
            const int bash = 92;
            var pathParts = path.Split((char) bash);
            if (pathParts.Length < 4) return false;     // C: - Users - <username> - OneDrive - maybeMore
            return (pathParts[3] == "OneDrive");
        }

        #endregion

        #region InitProperties - validation

        public static bool validProductName(SessionDTO session)
        {
            var inpProdName = session[_propMSI_ProductName];
            if (_productNameList.Any(x => x == inpProdName)) return true;
            return reportError(session, _sts_BAD_PRODUCTNAME,
                $"Unsupported ProductName: {inpProdName}");
        }

        public static bool validInstallType(SessionDTO session)
        {
            var inpInstallType = session[_propRISA_INSTALL_TYPE];
            if (_insTypeList.Any(x => x == inpInstallType)) return true;
            return reportError(session, _sts_BAD_INSTALLTYPE,
                $"Unsupported Install Type: {inpInstallType}");
        }
        public static bool processProductVersion(SessionDTO session)
        {
            var inpVersion = session[_propMSI_ProductVersion];
            _versionParts = inpVersion.Split('.');
            var partCt = _versionParts.Length;
            if (partCt == 3 || partCt == 4) return true;
            return reportError(session, _sts_BAD_PRODUCTVERSION,
                $"Invalid ProductVersion: {inpVersion}, require 3 or 4 part version");
        }

        private static bool reportError(SessionDTO session, string statusCode, string statusText)
        {
            session[_propRISA_STATUS_CODE] = statusCode;
            session[_propRISA_STATUS_TEXT] = statusText;
            sessLog(session, statusText);
            return false;
        }

        private static void sessLog(Session session, string msg)
        {
            session.Log($"InitProperties: {msg}");
        }

        private static void sessLog(SessionDTO sessDTO, string msg)
        {
            sessDTO.Log($"InitProperties: {msg}");
        }


        #endregion

        #region Helpers

        private static string displayedInstallType(string installType)
        {
            // note the space before " Demo" - resulting usage: 'RISA-3D 19.0' or 'RISA-3D 19.0 Demo'
            return installType == _insTypeDemo ? $" {_insTypeDemo}" : string.Empty;
        }

        #endregion
    }
}
