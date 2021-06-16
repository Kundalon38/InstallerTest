using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using RISA_CustomActionsLib;
using RISA_CustomActionsLib.Models.Linked;

namespace Silent_PreInstall_TestLib.Tests
{
    [TestClass, TestCategory("SiPreInstall")]
    public class SiPreInstall_Tests : TestBase
    {
        // "Testing" (inspection, really) is done by looking at the int result
        //   but also by viewing the Console Log
        // This set of tests depends on varying outer conditions:
        // - RISA-3D to be installed (or not installed)
        // - with many variations - see the text matrix xlsx

        [TestMethod]
        public void No_InsDir_Specified_OK()
        {
            expecting(HasPrivs, loc()); // require privs for install to work

            var cmdLine = $@"{exeShortFn} /qn";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "17.0.0"
            };
            var iSts = ProgramClass.Main(btd, new ConsoleLog());
            Assert.IsTrue(iSts == CustomActions._ists_SILENT_OK);
        }

        [TestMethod]
        public void InsDir_isIn_ProgramFiles_Fail()
        {
            expecting(HasPrivs, loc()); // require privs for install to work

            var propStr = propKvP(_propInsDir, @"C:\Program Files\Bob");
            var cmdLine = $@"{exeShortFn} /qn {propStr}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "17.0.0"
            };
            var iSts = ProgramClass.Main(btd, new ConsoleLog());
            Assert.IsTrue(iSts == CustomActions._ists_SILENT_OK);
        }
    }
}
