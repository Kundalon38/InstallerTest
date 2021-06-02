using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using RISA_CustomActionsLib.Extensions;
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
            return SilentValidate();
        }

        public static ActionResult SilentValidate(BootstrapperTestData testData = null)
        {
            // cull out for testing
            //_doTrace = true;    // set False for production use; use CmdLine LOG instead

            const string methodName = "SilentValidate";
            var bootData = BootstrapperData.FindBootstrapperFromCA(testData);
            if (bootData == null) return ActionResult.Success;
            if (!bootData.IsSilent) return ActionResult.Success;

            var validParse = bootData.ParseCmdLine();
            _log = new SiLog(bootData.LogFileName, false);

            foreach (var err in bootData.ErrorList) _log.Write(methodName, err.Text);
            if (!validParse || bootData.ErrorList.Any(x => x.IsFatal)) return ActionResult.Failure;

            bootData.ErrorList.Clear();
            var validProduct = bootData.ValidateProduct();
            var validVersion = bootData.ValidateVersion();
            foreach (var err in bootData.ErrorList) _log.Write(methodName, err.Text);
            if (!validProduct || validVersion) return ActionResult.Failure;

            bootData.ErrorList.Clear();
            var validProperties = bootData.ValidatePropertyValues();
            foreach (var err in bootData.ErrorList) _log.Write(methodName, err.Text);
            if (!validProperties) return ActionResult.Failure;

            bootData.ErrorList.Clear();
            var insProductList = findInstalledProducts(bootData.ProductName);
            var installOldOverNew = bootData.IsInstallOldOverNew(insProductList);
            foreach (var err in bootData.ErrorList) _log.Write(methodName, err.Text);
            if (installOldOverNew) return ActionResult.Failure;

            if (!anyRISAproductsActive()) return ActionResult.Success;

            bootData.ErrorList.Clear();
            var errmsg = $"Other RISA/ADAPT product(s) are active.  Please save your work, close them and restart this installer.";
            bootData.ErrorList.Add(new SiError(errmsg));
            foreach (var err in bootData.ErrorList) _log.Write(methodName, err.Text);
            return ActionResult.Failure;
        }

        private static SiLog _log;
    }
}
