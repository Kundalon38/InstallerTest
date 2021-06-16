using Microsoft.Deployment.WindowsInstaller;
using RISA_CustomActionsLib.Models.Linked;

namespace RISA_CustomActionsLib.Models
{
    // used by testing to look at everything
    // is similar to a Tuple<>, which isn't available in .NET 4.5
    //
    public class SilentResult
    {
        public ActionResult Result { get; }
        public BootstrapperData BooData { get; }

        private SilentResult(ActionResult ar, BootstrapperData booData)
        {
            Result = ar;
            BooData = booData;
        }

        public bool IsOK => Result == ActionResult.Success;
        public bool IsFail => Result == ActionResult.Failure;

        public static SilentResult OK(BootstrapperData booData)
        {
            return new SilentResult(ActionResult.Success, booData);
        }

        public static SilentResult Fail (BootstrapperData booData)
        {
            return new SilentResult(ActionResult.Failure, booData);
        }
    }
}
