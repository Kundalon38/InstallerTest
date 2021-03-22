using Microsoft.Deployment.WindowsInstaller;
using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Win32;

namespace RISA_CustomActionsLib
{
    // note that this this lib is built against .NET 4.5
    // - very low bar, most / all customer machines wil have this,
    //   eliminating the need to provision the customer machine for this lib
    //
    public partial class CustomActions
    {
        #region DetectRoaming

        // Deprecated - functionality is subsumed in InitProperties.assignDocumentPath()

        [CustomAction]
        public static ActionResult DetectRoaming(Session session)
        {
            const string installTypePropertyName = "INSTALL_TYPE";
            const string outputPropertyName = "USERFILES_RISA";
            
            string installTypeProperty = session[installTypePropertyName];
            string folderName = (installTypeProperty == "Demo") ? "RISADemo" : "RISA";

            string outputDirIfRoaming = @"C:\" + folderName;        // TODO hardwiring C: is wrong
            //
            // typical myDocsPath:      C:\Users\<username>\Documents
            // typical userProfilePath: C:\Users\<username>
            //
            var myDocsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            var isRoaming = myDocsPath.StartsWith(@"\\");       // roaming if a unc path
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

            session[outputPropertyName] = isRoaming
                ? outputDirIfRoaming
                : Path.Combine(myDocsPath, folderName);

            return ActionResult.Success;
        }

        #endregion

        #region Stop / Start RISA32 Service

        //
        // The Services applet says the Name + Description are "RISA32 License Manager Service";
        //      the service name, used below, is RISA32
        //
        // Code was cribbed from LicensingDashboard, where it's implemented using async/await,
        //      however that version and this one simply:
        //          issue stop/start service,
        //          then: sleep / poll <-repeat until service is in desired state.
        //      Re-implement here using old school Thread.Sleep to avoid client interfacing problems with async/await
        //
        private const string _risaServiceName = "RISA32";
        private const int _sleep_millisecs = 500;
        private const int _num_SleepsPerSecond = 2;
        private const int _maxWaitForService_Seconds = 60;

        [CustomAction]
        public static  ActionResult ServiceControlStopRISA32(Session session)
        {
            session.Log("ServiceControlStopRISA32");
            var services = ServiceController.GetServices();

            var risaSvc = services.FirstOrDefault(x => x.ServiceName == _risaServiceName);

            if (risaSvc == null) return ActionResult.Success;                                   // service isn't installed
            if (risaSvc.Status != ServiceControllerStatus.Running) return ActionResult.Success; // service isn't running
            //
            // stop service
            //
            try
            {
                session.Log("Attempting to stop RISA32");
                risaSvc.Stop();             // asynchronous, call returns before service actually stops

                for (var nWaits = 0; nWaits < _maxWaitForService_Seconds * _num_SleepsPerSecond; nWaits++)
                {
                    Thread.Sleep(_sleep_millisecs);
                    risaSvc.Refresh();
                    if (risaSvc.Status == ServiceControllerStatus.Stopped) return ActionResult.Success;
                    session.Log("RISA32 service not stopped yet");
                }
            }
            catch
            {
                session.Log("Stopping RISA32 service: threw an exception");
            }
            session.Log("ServiceControlStopRISA32: Returning failue");
            return ActionResult.Failure;
        }

        [CustomAction]
        public static ActionResult ServiceControlStartRISA32(Session session)
        {
            // always return Success, and is fire n forget - don't wait for service to fully start
            //
            var services = ServiceController.GetServices();

            var risaSvc = services.FirstOrDefault(x => x.ServiceName == _risaServiceName);

            if (risaSvc == null) return ActionResult.Success;                                   // service isn't installed
            if (risaSvc.Status == ServiceControllerStatus.Running) return ActionResult.Success; // service is already running
            //
            // start service
            //
            try
            {
                session.Log("Attempting to start RISA32");
                risaSvc.Start();             // asynchronous, call returns before service actually starts

            //    for (var nWaits = 0; nWaits < _maxWaitForService_Seconds * _num_SleepsPerSecond; nWaits++)
            //    {
            //        Thread.Sleep(_sleep_millisecs);
            //        if (risaSvc.Status == ServiceControllerStatus.Running) return ActionResult.Success;
            //    }
            }
            catch
            {
            }
            return ActionResult.Success;
        }

        #endregion


        #region InitProperties

        [CustomAction]
        public static ActionResult InitProperties(Session session)
        {
            string msgText = null;
            var actionResult = ActionResult.Success;
            try
            {
                if (!validProductName(session)) return ActionResult.Failure;
                if (!processProductVersion(session)) return ActionResult.Failure;
                if (!validInstallType(session)) return ActionResult.Failure;

                assignVersionBasedProperties(session);
                assignRemainingIdentityBasedProperties(session);
                assignDocumentPath(session);
                assignDefaultLicenseType(session);

                if (_versionParts.Length == 3)
                {
                    session[_propRISA_STATUS_CODE] = _sts_WARN_VERSION3;
                    msgText = "ProductVersion should have 4 version parts";
                }
                else
                {
                    session[_propRISA_STATUS_CODE] = _sts_OK;
                    msgText = "Success";
                }
            }
            catch (Exception e)
            {
                session[_propRISA_STATUS_CODE] = _sts_EXCP;
                msgText = $"{e.Message}{Environment.NewLine}{e.StackTrace}";
                actionResult = ActionResult.Failure;
            }
            finally
            {
                sessLog(session, msgText);
            }
            return actionResult;
        }

        #region InitProperties - assignment

        private static string[] _versionParts;  // set in processProductVersion()

        private static void assignVersionBasedProperties(Session session)
        {
            // versions are (MS terminology): major.minor.build.revision
            //
            // version2 = major.minor version parts
            var version2 = $"{_versionParts[0]}.{_versionParts[1]}";
            session[_propRISA_PRODUCT_VERSION2] = version2;

            // prodDisplayName eventually = eg RISA-3D 19.0  or  RISA-3D Demo 19.0
            var prodDisplayName = session[_propMSI_ProductName];
            if (session[_propRISA_INSTALL_TYPE] == _insTypeDemo) prodDisplayName += $" {_insTypeDemo}";
            prodDisplayName += $" {version2}";
            session[_propRISA_PRODUCT_TITLE2_INSTYPE] = prodDisplayName;

            // build.revision version parts; allow that revision may be omitted
            var vers4 = "0";
            if (_versionParts.Length == 4) vers4 = _versionParts[3];
            session[_propRISA_PRODUCT_VERSION34] = $"{_versionParts[2]}.{vers4}";
        }

        private static void assignRemainingIdentityBasedProperties(Session session)
        {
            var prodNdx = Array.IndexOf(_productNameList, session[_propMSI_ProductName]);
            var updateSfx = session[_propRISA_INSTALL_TYPE] == _insTypeDemo
                                    ? $"_{_insTypeDemo}" : "_SA";

            // this gets written to the registry, generate the value here, such as eg "UpdateData3D_SA"
            session[_propRISA_UPDATE_DATA_VALUE] = $"UpdateData{_updateProductAbbrevs[prodNdx]}{updateSfx}";

        }

        private static void assignDefaultLicenseType(Session session)
        {

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

        private static void assignDocumentPath(Session session)
        {
            // typical myDocsPath:      C:\Users\<username>\Documents
            // typical userProfilePath: C:\Users\<username>
            //
            var myDocsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            var isRoaming = myDocsPath.StartsWith(@"\\");       // roaming if a unc path
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
            if (isRoaming)
            {
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
            session["USERFILES_RISA"] = docsPath;           // deprecated
        }

        #endregion

        #region InitProperties - validation

        private static bool validProductName(Session session)
        {
            var inpProdName = session[_propMSI_ProductName];
            if(_productNameList.Any(x => x == inpProdName)) return true;
            return reportError(session, _sts_BAD_PRODUCTNAME,
                $"Unsupported ProductName: {inpProdName}");
        }

        private static bool validInstallType(Session session)
        {
            var inpInstallType = session[_propRISA_INSTALL_TYPE];
            if (_insTypeList.Any(x => x == inpInstallType)) return true;
            return reportError(session, _sts_BAD_INSTALLTYPE,
                $"Unsupported Install Type: {inpInstallType}");
        }
        private static bool processProductVersion(Session session)
        {
            var inpVersion = session[_propMSI_ProductVersion];
            _versionParts = inpVersion.Split('.');
            var partCt = _versionParts.Length;
            if (partCt == 3 || partCt == 4) return true;
            return reportError(session, _sts_BAD_PRODUCTVERSION,
                $"Invalid ProductVersion: {inpVersion}, require 3 or 4 part version");
        }

        private static bool reportError(Session session, string statusCode, string statusText)
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


        #endregion


        #endregion
    }
}
