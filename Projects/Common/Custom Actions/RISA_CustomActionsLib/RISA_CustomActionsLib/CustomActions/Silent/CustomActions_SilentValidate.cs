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
            //_doTrace = true;    // set False for production use; use CmdLine LOG instead

            const string methodName = "SilentValidate";
            var bootData = BootstrapperData.FindBootstrapperFromCA();
            if (bootData == null) return ActionResult.Success;
            if (!bootData.IsSilent) return ActionResult.Success;

            var validParse = bootData.ParseCmdLine();
            _log = new SiLog(bootData.LogFileName, false);

            foreach(var err in bootData.ErrorList) _log.Write(methodName, err.Text);
            if (!validParse || bootData.ErrorList.Any(x => x.IsFatal)) return ActionResult.Failure;
            bootData.ErrorList.Clear();

            var validProduct = bootData.ValidateProduct();
            var validVersion = bootData.ValidateVersion();
            foreach (var err in bootData.ErrorList) _log.Write(methodName, err.Text);
            if (!validProduct || validVersion) return ActionResult.Failure;
            bootData.ErrorList.Clear();

            var validProperties = bootData.ValidateProperties();

            foreach (var err in bootData.ErrorList) _log.Write(methodName, err.Text);
            if (!validProduct) return ActionResult.Failure;
            bootData.ErrorList.Clear();

        }
        private static SiLog _log;

    }
}
