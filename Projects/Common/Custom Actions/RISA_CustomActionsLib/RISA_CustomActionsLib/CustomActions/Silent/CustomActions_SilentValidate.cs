using Microsoft.Deployment.WindowsInstaller;
using System.Linq;
using RISA_CustomActionsLib.Models;
using RISA_CustomActionsLib.Models.Linked;
using BootstrapperData = RISA_CustomActionsLib.Models.Linked.BootstrapperData;

namespace RISA_CustomActionsLib
{
    public partial class CustomActions
    {
        [CustomAction]
        public static ActionResult SilentValidate(Session session)
        {
            return SilentValidate().Result;
        }

        public static SilentResult SilentValidate(BootstrapperTestData testData = null, ISiLog logger = null)
        {
            // cull out for testing
            //_doTrace = true;    // set False for production use; use CmdLine LOG instead

            const string methodName = "SilentValidate";
            var bootData = BootstrapperData.FindBootstrapperFromCA(testData);
            if (bootData == null) return SilentResult.OK(bootData);
            if (!bootData.IsSilent) return SilentResult.OK(bootData);

            var validParse = bootData.ParseCmdLine();
            if (logger == null) _log = new SiLog(bootData.LogFileName, false);
            else _log = logger;

            foreach (var err in bootData.ErrorList) _log.Write(methodName, err.Text);
            if (!validParse || bootData.ErrorList.Any(x => x.IsFatal)) return SilentResult.Fail(bootData);

            bootData.ErrorList.Clear();
            var validProduct = bootData.ValidateProduct();
            var validVersion = bootData.ValidateVersion();
            foreach (var err in bootData.ErrorList) _log.Write(methodName, err.Text);
            if (!validProduct || !validVersion) return SilentResult.Fail(bootData);

            bootData.ErrorList.Clear();
            var validProperties = bootData.ValidatePropertyValues();
            foreach (var err in bootData.ErrorList) _log.Write(methodName, err.Text);
            if (!validProperties) return SilentResult.Fail(bootData);

            bootData.ErrorList.Clear();
            var insProductList = findInstalledProducts(bootData.ProductName);
            var installOldOverNew = bootData.IsInstallOldOverNew(insProductList);
            foreach (var err in bootData.ErrorList) _log.Write(methodName, err.Text);
            if (installOldOverNew) return SilentResult.Fail(bootData);

            bootData.ErrorList.Clear();
            SilentResult retSts;
            if (anyRISAproductsActive())
            {
                var errmsg =
                    $"Other RISA/ADAPT product(s) are active.  Please save your work, close them and restart this installer.";
                bootData.ErrorList.Add(new SiError(errmsg));
                retSts = SilentResult.Fail(bootData);
            }
            else
            {
                bootData.ErrorList.Add(new SiError("Successful input validation",false));
                retSts = SilentResult.OK(bootData);
            }
            foreach (var err in bootData.ErrorList) _log.Write(methodName, err.Text);
            return retSts;
        }

        private static ISiLog _log;
    }
}
