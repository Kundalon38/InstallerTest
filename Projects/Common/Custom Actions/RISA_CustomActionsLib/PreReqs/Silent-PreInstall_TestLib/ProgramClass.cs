using System;
using System.Collections.Generic;
using System.Linq;
using RISA_CustomActionsLib;
using RISA_CustomActionsLib.Models.Linked;

namespace Silent_PreInstall_TestLib
{
    public class ProgramClass
    {
        public static int Main(BootstrapperTestData testData = null, ISiLog logger = null)
        {
            // culled out for testing - code is then moved into Silent-PreInstall console app
            // this repeats some of Silent-Validate's logic to get things set up,
            //  but pares down the validation to get on with the main purpose: remove an installed product if needed

            var bootData = BootstrapperData.FindBootstrapperFromExe(testData);
            if (bootData == null) return CustomActions._ists_SILENT_OK;
            if (!bootData.IsSilent) return CustomActions._ists_SILENT_OK;

            var validParse = bootData.ParseCmdLine();
            if (logger == null) _log = new SiLog(bootData.LogFileName, true);
            else _log = logger;

            foreach (var err in bootData.ErrorList) _log.Write(_methodName, err.Text);
            if (!validParse || bootData.ErrorList.Any(x => x.IsFatal)) return CustomActions._ists_SILENT_ERR;

            bootData.ErrorList.Clear();
            var validProperties = bootData.ValidatePropertyValues();            // reads ini file if exists
            foreach (var err in bootData.ErrorList) _log.Write(_methodName, err.Text);
            if (!validProperties) return CustomActions._ists_SILENT_ERR;

            bootData.ErrorList.Clear();
            var insProductList = InstalledProductList.FindInstalledProducts(bootData.ProductName);
            if (insProductList.Count == 0) return CustomActions._ists_SILENT_OK;    // no installed products ==> go home

            var returnStatus = CustomActions._ists_SILENT_OK;
            var tbRemoved = new List<InstalledProduct>();
            try
            {
                //
                // normalize the two directory strings, sometimes there's a trailing bash, sometimes not
                //
                var insDirKvp = bootData.CmdLineProperties[BootstrapperDataCommon._propInsDir];
                var userSpecifiedInsDir = insDirKvp?.PropValue;
                var normalizedAppDir = getInstallDir(userSpecifiedInsDir,CustomActions._insTypeStandalone).EnsureTrailingBash();
                _log.Write(_methodName, $"Installation directory resolved to: {normalizedAppDir}");
                foreach (var insProd in insProductList)
                {
                    if (string.Equals(insProd.TargetDir.EnsureTrailingBash(), normalizedAppDir,
                        StringComparison.CurrentCultureIgnoreCase)) tbRemoved.Add(insProd);
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
                    _log.Write(_methodName,"No earlier products were removed");
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
