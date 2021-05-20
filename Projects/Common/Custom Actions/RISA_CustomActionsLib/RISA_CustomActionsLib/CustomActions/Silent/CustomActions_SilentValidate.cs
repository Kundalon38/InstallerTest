using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Diagnostics;
using System.IO;
using RISA_CustomActionsLib.Extensions;
using RISA_CustomActionsLib.Models;

namespace RISA_CustomActionsLib
{
    public partial class CustomActions
    {
        [CustomAction]
        public static ActionResult SilentValidate(Session session)
        {
            _doTrace = true;                // TODO set False for production use

            const string methodName = "SilentValidate";
            var bootData = FindBootstrapper(methodName);

            if(bootData != null) Trace(methodName, bootData.ToString());
            return ActionResult.Success;
        }
    }
}
