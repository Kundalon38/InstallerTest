using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.WindowsInstaller;

namespace RISA_CustomActionsLib.Models.Linked
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
