using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RISA_CustomActionsLib.Models.Linked;

namespace RISA_CustomActionsLib.Test
{
    [TestClass]
    public class SiValidate_Tests : TestBase
    {
        [TestMethod, TestCategory("SiValidate")]
        public void CmdLine_Loud_OK()
        {
            var cmdLine = $@"install_3d_1900.exe";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine
            };
            var siRes = CustomActions.SilentValidate(btd);

            expecting(bd.IsSilent);
            bd.ParseCmdLine();
            expecting(bd.CmdLineProperties.Count == 1);
            expecting(bd.CmdLineProperties[0].PropName == myProp);
            expecting(bd.CmdLineProperties[0].PropValue == myPropValue);
            Assert.IsTrue(true);
        }


    }
}
