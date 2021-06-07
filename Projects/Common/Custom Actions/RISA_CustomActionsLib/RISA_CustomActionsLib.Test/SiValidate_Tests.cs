using System;
using System.Collections.Generic;
using System.IO;
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

            expecting(!siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsOK);
        }

        #region Logging

        [TestMethod, TestCategory("SiValidate")]
        public void Log_DESKTOP_keyword_OK()
        {
            var propStr = propKvP(_propLogFile, "DESKTOP");
            var cmdLine = $@"{exeShortFn} /qn {propStr}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd);

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsOK);
        }

        [TestMethod, TestCategory("SiValidate")]
        public void Log_SpecificFn_OK()
        {
            var fn = Path.Combine(deskTopDir, "My.Log");
            var propStr = propKvP(_propLogFile, fn);
            var cmdLine = $@"{exeShortFn} /qn {propStr}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd);

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsOK);
        }

        [TestMethod, TestCategory("SiValidate")]
        public void Log_Nothing_OK()
        {
            var cmdLine = $@"{exeShortFn} /qn";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd);

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsOK);
        }

        #endregion

        #region Validate Product

        [TestMethod, TestCategory("SiValidate")]
        public void Validate_KnownProduct_OK()
        {
            var cmdLine = $@"{exeShortFn} /qn";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsOK);
        }

        [TestMethod, TestCategory("SiValidate")]
        public void Validate_DemoProduct_Fail()
        {
            var cmdLine = $@"{exeShortFn} /qn";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = $"{_prod3D} Demo",
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsFail);
        }

        [TestMethod, TestCategory("SiValidate")]
        public void Validate_UnSupportedProduct_Fail()
        {
            var cmdLine = $@"{exeShortFn} /qn";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = $"RISA-Revit 2021 Link",
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsFail);
        }

        #endregion

        #region Validate Version

        [TestMethod, TestCategory("SiValidate")]
        public void Validate_MissingVersion_Fail()
        {
            var cmdLine = $@"{exeShortFn} /qn";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsFail);
        }

        [TestMethod, TestCategory("SiValidate")]
        public void Validate_PartialVersion_OK()
        {
            var cmdLine = $@"{exeShortFn} /qn";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsOK);
        }

        [TestMethod, TestCategory("SiValidate")]
        public void Validate_BadVersionChars_Fail()
        {
            var cmdLine = $@"{exeShortFn} /qn";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.XYZ"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsFail);
        }

        #endregion

        #region Region Code

        [TestMethod, TestCategory("SiValidate")]
        public void RegionCode_TooBig_Fail()
        {
            var propStr = propKvP(_propRegion, "BAD");
            var cmdLine = $@"{exeShortFn} /qn {propStr}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsFail);
        }

        [TestMethod, TestCategory("SiValidate")]
        public void RegionCode_BadChar_Fail()
        {
            var propStr = propKvP(_propRegion, "B");
            var cmdLine = $@"{exeShortFn} /qn {propStr}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsFail);
        }

        [TestMethod, TestCategory("SiValidate")]
        public void RegionCode_Valid_ForProduct_OK()
        {
            var propStr = propKvP(_propRegion, "1");
            var cmdLine = $@"{exeShortFn} /qn {propStr}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prodCN,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsOK);
        }
        [TestMethod, TestCategory("SiValidate")]
        public void RegionCode_InValid_ForProduct_Fail()
        {
            var propStr = propKvP(_propRegion, "2");
            var cmdLine = $@"{exeShortFn} /qn {propStr}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prodCN,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsFail);
        }

        #endregion

        #region Auto Update

        [TestMethod, TestCategory("SiValidate")]
        public void UpdCode_Valid_OK()
        {
            var propStr = propKvP(_propUpdate, _ansYes);
            var cmdLine = $@"{exeShortFn} /qn {propStr}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prodCN,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsOK);
        }
        [TestMethod, TestCategory("SiValidate")]
        public void UpdCode_InValid_Fail()
        {
            var propStr = propKvP(_propUpdate, "Y");
            var cmdLine = $@"{exeShortFn} /qn {propStr}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prodCN,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsFail);
        }

        #endregion

        #region License Type

        [TestMethod, TestCategory("SiValidate")]
        public void LicenseType_Omit_Fail()
        {
            var propStr = propKvP(_propLicType, "");
            var cmdLine = $@"{exeShortFn} /qn {propStr}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prodCN,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsFail);
        }
        [TestMethod, TestCategory("SiValidate")]
        public void LicenseType_Misspell_Fail()
        {
            var propStr = propKvP(_propLicType, "Sub");
            var cmdLine = $@"{exeShortFn} /qn {propStr}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prodCN,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsFail);
        }
        [TestMethod, TestCategory("SiValidate")]
        public void LicenseType_Valid_OK()
        {
            var propStr = propKvP(_propLicType, _ltCloud);
            var cmdLine = $@"{exeShortFn} /qn {propStr}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prodCN,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsOK);
        }

        #endregion

        #region Install Old over New

        [TestMethod, TestCategory("SiValidate")]
        public void Old_Over_New_Fail()
        {
            // NOTE: RISA-3D > 17.0 must be installed

            var cmdLine = $@"{exeShortFn} /qn";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "17.0.0"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsFail);
        }

        #endregion

        #region Install Old over New

        [TestMethod, TestCategory("SiValidate")]
        public void Any_RISA_Products_Running()
        {
            // NOTE: You have to have a RISA product running

            var cmdLine = $@"{exeShortFn} /qn";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prodCN,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsFail);
        }

        #endregion

    }
}
