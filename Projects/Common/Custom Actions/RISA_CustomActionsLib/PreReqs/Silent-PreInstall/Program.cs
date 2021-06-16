using System;
using System.Collections.Generic;
using System.Linq;
using RISA_CustomActionsLib;
using RISA_CustomActionsLib.Models.Linked;

namespace Silent_PreInstall
{
    // Silent_PreInstall's only function is:
    // - figure out where product is going to be installed to, detect if it will collide with an existing product
    //   and remove the existing product if this is the case
    // - an early test is to determine if this is a silent install.  If it isn't, drop thru.
    // - int status is returned, and assigned to a property by AI, which is read downstream.
    //   It's critical to return 0 for success (an MSI convention).
    //
    // This application must be built x64,
    //  at least for SpecialFolders.ProgramFiles to resolve correctly
    //
    class Program
    {
        static int Main(string[] args)
        {
            // code below was developed from test location: Silent_PreInstall_TestLib.ProgramClass.Main()
            BootstrapperData bootData = null;
            InstalledProductList insProductList = null;

            try
            {
                bootData = BootstrapperData.FindBootstrapperFromExe();
                if (bootData == null) return CustomActions._ists_SILENT_OK;
                if (!bootData.IsSilent) return CustomActions._ists_SILENT_OK;

                var validParse = bootData.ParseCmdLine();
                _log = new SiLog(bootData.LogFileName, true);

                foreach (var err in bootData.ErrorList) _log.Write(_methodName, err.Text);
                if (!validParse || bootData.ErrorList.Any(x => x.IsFatal)) return CustomActions._ists_SILENT_ERR;

                bootData.ErrorList.Clear();
                var validProperties = bootData.ValidatePropertyValues();            // reads ini file if exists
                foreach (var err in bootData.ErrorList) _log.Write(_methodName, err.Text);
                if (!validProperties) return CustomActions._ists_SILENT_ERR;

                bootData.ErrorList.Clear();
                insProductList = InstalledProductList.FindInstalledProducts(bootData.ProductName);
                if (insProductList.Count == 0) return CustomActions._ists_SILENT_OK;    // no installed products ==> go home
            }
            catch (Exception e)
            {
                _log.Write(_methodName, $"{e.Message}{Environment.NewLine}{e.StackTrace}");
                return CustomActions._ists_SILENT_ERR;
            }

            var returnStatus = CustomActions._ists_SILENT_OK;
            var tbRemoved = new List<InstalledProduct>();
            try
            {
                //
                // normalize the two directory strings, sometimes there's a trailing bash, sometimes not
                //
                var insDirKvp = bootData.CmdLineProperties[BootstrapperDataCommon._propInsDir];
                var userSpecifiedInsDir = insDirKvp?.PropValue;
                var normalizedAppDir = getInstallDir(userSpecifiedInsDir, CustomActions._insTypeStandalone).EnsureTrailingBash();
                _log.Write(_methodName, $"Installation directory resolved to: {normalizedAppDir}");

                foreach (var insProd in insProductList)
                {
                    if(insProd.TargetDir.EnsureTrailingBash().IsEqIgnoreCase(normalizedAppDir)) tbRemoved.Add(insProd);
                }

                foreach (var insProd in tbRemoved)
                {
                    _log.Write(_methodName, $"Attempting uninstall of {insProd.ProductName} {insProd.ProductVersionStr} in {normalizedAppDir}");
                    insProd.UnInstall();
                }
            }
            catch (Exception e)
            {
                _log.Write(_methodName, $"{e.Message}{Environment.NewLine}{e.StackTrace}");
                returnStatus = CustomActions._ists_SILENT_ERR;
            }
            finally
            {
                if (tbRemoved.Count == 0)
                    _log.Write(_methodName, "No earlier products were removed");
                else
                {
                    // check that removal worked. Typically msiexec uninstall will fail silently (won't appear in catch block)
                    var insProductList2 = InstalledProductList.FindInstalledProducts(bootData.ProductName);
                    if (insProductList.Count == insProductList2.Count)
                    {
                        _log.Write(_methodName, $"Could not uninstall earlier version of {bootData.ProductName}");
                        _log.Write(_methodName, $"Please uninstall it by hand, or choose a different installation directory");
                        returnStatus = CustomActions._ists_SILENT_ERR_REMOVE_INSTALLED_PRODUCT;
                    }
                    else _log.Write(_methodName, $"Uninstall Succeeded");
                }
            }
            return returnStatus;
        }
        private static ISiLog _log;
        private const string _methodName = "Silent-PreInstall";

        public static string getInstallDir(string userRequestedInsDir, string installType)
        {
            if (userRequestedInsDir == null) // installation dir defaulted, TODO log decision
            {
                return CustomActions.isAproblemProfile()
                    ? CustomActions.altInstallDir(installType)
                    : CustomActions.pgmFilesInsDir(installType);
            }
            // verify that specified ins dir is not a child of C:\Program Files
            //  I believe this is because both ins dir and documents go here (they're unfortunately conflated)
            //   application will fail if it tries to write (documents) to C:\Program Files
            //
            var pgmFilesDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            if (userRequestedInsDir.StartsWith(pgmFilesDir, StringComparison.CurrentCultureIgnoreCase))
            {
                return CustomActions.altInstallDir(installType);
            }

            return userRequestedInsDir;
        }
     }
}
