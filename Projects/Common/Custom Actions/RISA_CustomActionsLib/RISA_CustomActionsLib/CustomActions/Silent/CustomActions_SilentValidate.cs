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
            // culled out for testing

            const string methodName = "Silent-Validate";
            var bootData = BootstrapperData.FindBootstrapperFromCA(testData);
            if (bootData == null) return SilentResult.OK(bootData);
            if (!bootData.IsSilent) return SilentResult.OK(bootData);

            ISiLog log;
            var validParse = bootData.ParseCmdLine();
            if (logger == null) log = new SiLog(bootData.LogFileName, false);
            else log = logger;

            foreach (var err in bootData.ErrorList) log.Write(methodName, err.Text);
            if (!validParse || bootData.ErrorList.Any(x => x.IsFatal)) return SilentResult.Fail(bootData);

            bootData.ErrorList.Clear();
            var validProduct = bootData.ValidateProduct();
            var validVersion = bootData.ValidateVersion();
            foreach (var err in bootData.ErrorList) log.Write(methodName, err.Text);
            if (!validProduct || !validVersion) return SilentResult.Fail(bootData);

            bootData.ErrorList.Clear();
            var validProperties = bootData.ValidatePropertyValues();
            foreach (var err in bootData.ErrorList) log.Write(methodName, err.Text);
            if (!validProperties) return SilentResult.Fail(bootData);

            bootData.ErrorList.Clear();
            var insProductList = InstalledProductList.FindInstalledProducts(bootData.ProductName);
            var installOldOverNew = bootData.IsInstallOldOverNew(insProductList);
            foreach (var err in bootData.ErrorList) log.Write(methodName, err.Text);
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
            foreach (var err in bootData.ErrorList) log.Write(methodName, err.Text);
            return retSts;
        }
    }
}
