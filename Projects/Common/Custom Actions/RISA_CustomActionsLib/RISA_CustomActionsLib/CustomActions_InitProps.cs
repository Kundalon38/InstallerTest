//#define ATTACH_DEBUGGER
using System;
using System.Collections.Generic;
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
        //
        // return ActionResult.Success otherwise AI shows a (useless) generic failure screen
        // follow a call to these CustomActions with a dialog that's conditioned on RISA_STATUS_CODE<>"RISA_STS_OK"

        [CustomAction]
        public static ActionResult InitProperties(Session session)
        {
            const string methodName = "InitProperties";

#if ATTACH_DEBUGGER
            int processId = Process.GetCurrentProcess().Id;
            string message = string.Format("Init Properties: Please attach the debugger (elevated) to process [{0}].", processId);
            MessageBox.Show(message, "Debug");
#endif

            #region copy Session props to SessionDTO
            SessionDTO sessDTO;
            try
            {
                sessDTO = initProperties_getFromSession(session);
            }
            catch (Exception ex)
            {
                excpLog(session, methodName, ex);
                return ActionResult.Success;
            }
            #endregion

            initProperties(sessDTO);

            #region copy SessionDTO props back to Session
            try
            {
                initProperties_returnToSession(session, sessDTO);
            }
            catch (Exception ex)
            {
                excpLog(session, methodName, ex);
                return ActionResult.Success;
            }
            #endregion

            return ActionResult.Success;
        }

        #region InitProperties - Session / SessionDTO property copying

        private static SessionDTO initProperties_getFromSession(Session session)
        {
            var sessDTO = new SessionDTO(session.Log);
            // prop values set in aip
            copySinglePropFromSession(sessDTO, session, _propMSI_ProductName);
            copySinglePropFromSession(sessDTO, session, _propMSI_ProductVersion);
            copySinglePropFromSession(sessDTO, session, _propRISA_COMPANY_KEY);
            copySinglePropFromSession(sessDTO, session, _propRISA_INSTALL_TYPE);
            copySinglePropFromSession(sessDTO, session, _propRISA_REGISTRY_PRODUCT_NAME);
            //
            copySinglePropFromSession(sessDTO, session, _propRISA_CA_DEBUG,false);
            setupDebugIfRequested(session, sessDTO);
            //
            // prop values set by InitProperties
            copySinglePropFromSession(sessDTO, session, _propRISA_INSTALLED_PRODUCTS);
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

        private static void initProperties_returnToSession(Session session, SessionDTO sessDTO)
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
            //
            const string methodName = "initProperties_returnToSession";
            Trace(methodName, sessDTO.ToString());
        }

        #endregion

        #region initProperties - without Session wrapper, for unit testing

        public static void initProperties(SessionDTO sessDTO)
        {
            const string methodName = "initProperties";
            string msgText = null;
            try
            {
                if (!validProductName(sessDTO)) return;
                if (!processProductVersion(sessDTO)) return;
                if (!validInstallType(sessDTO)) return;
                if (!noRISAproductsAreRunning(sessDTO)) return;

                assignVersionBasedProperties(sessDTO);
                if(!serializeMatchingInstalledProducts(sessDTO)) return;
                assignRemainingIdentityBasedProperties(sessDTO);
                assignDocumentPath(sessDTO);
                assignDefaultLicenseType(sessDTO);

                sessDTO[_propRISA_STATUS_CODE] = _sts_OK;
                sessDTO[_propRISA_STATUS_TEXT] = "Success"; ;
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

        #region InitProperties - ensure no products are running

        public static bool noRISAproductsAreRunning(SessionDTO sessDTO)
        {
            // exclude risaservice because 1) it's not a product and 2) probing it may throw Access denied excp
            // shutting down risaservice is directly handled by those installers that need to update it

            // merely testing the process names is shaky, esp a common name like "adapt"
            // - deeper probing is possible if tricky, see the "stash for possible future need" region below

            var allProcessNamesLC = Process.GetProcesses().Select(x => x.ProcessName.ToLower());
            var risaProcessNamesLC = allProcessNamesLC
                .Where(x => (x.StartsWith("risa") || x.StartsWith("adapt")) && x != "risaservice");

            if (!risaProcessNamesLC.Any()) return true;
            const string methodName = "noRISAproductsAreRunning";
            return reportError(sessDTO, methodName, _sts_ERR_PRODUCT_ACTIVE,
                $"Other RISA/ADAPT product(s) are active.  Please save your work, close them and restart this installer.");

            #region stash for possible future need

            // deeper checks could be done, by getting the exe's FileVersionInfo block, but watch out for excps:
            // code lifted from Licensing Dashboard, where it never really was used:
            //        try
            //        {
            //            if (rp.MainModule == null) continue;
            //            var fvi = rp.MainModule.FileVersionInfo;
            //            var uniqueProductNameSnippet =
            //                App.UniqueProductNameSnippets.FirstOrDefault(snippet => fvi.ProductName.Contains(snippet));
            //            if (uniqueProductNameSnippet == null) continue;
            //            knownRISAprocesses.Add(rp);
            //        }
            //        catch(Exception ex)
            //        {
            //            // a variety of excps are possible, including, program exit (why is it still in the processs list?)
            //            //  and Win32 excp: 32 bit process being queried by a 64 bit process (this one), and vice versa.
            //            // RISA Section is 32 bit, but all other contemporary versions are 64 bit.
            //            // Tools such as RISA Change License Type can be 32 bits and will come here.
            //            // Allow that there could be old 32 bit stuff out there.
            //            var processName = rp.ProcessName ?? string.Empty;
            //            App.Trace.Excp(ex,$"Process: {processName}; loc {loc()}");
            //        }

            #endregion
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
            const string methodName = "validProductName";
            return reportError(session, methodName,_sts_BAD_PRODUCTNAME,
                $"Unsupported ProductName: {inpProdName}");
        }

        public static bool validInstallType(SessionDTO session)
        {
            var inpInstallType = session[_propRISA_INSTALL_TYPE];
            if (_insTypeList.Any(x => x == inpInstallType)) return true;
            const string methodName = "validInstallType";
            return reportError(session, methodName, _sts_BAD_INSTALLTYPE,
                $"Unsupported Install Type: {inpInstallType}");
        }
        public static bool processProductVersion(SessionDTO session)
        {
            var inpVersion = session[_propMSI_ProductVersion];
            _versionParts = inpVersion.Split('.');
            var partCt = _versionParts.Length;
            if (partCt == 3 || partCt == 4) return true;
            const string methodName = "processProductVersion";
            return reportError(session, methodName, _sts_BAD_PRODUCTVERSION,
                $"Invalid ProductVersion: {inpVersion}, require 3 or 4 part version");
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
