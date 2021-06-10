using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RISA_CustomActionsLib.Models;
using RISA_CustomActionsLib.Models.Linked;

namespace RISA_CustomActionsLib.Test.SilentCAs_Tests
{
    [TestClass]
    public class SiInitProps_Tests : TestBase
    {
        [TestInitialize]
        public void TestInit()
        {
            _sessDTO_undefined_PreInstall_Result_property = new SessionDTO(Console.WriteLine)
            {
                // init property names - clone of initSessionDTO()
                // props set by installer

                [CustomActions._propAI_APPDIR] = _dummy,
                [CustomActions._propMSI_ProductName] = _risa3D,
                [CustomActions._propMSI_ProductVersion] = _risa3Dversion,
                [CustomActions._propRISA_COMPANY_KEY] = "RISA Technologies",
                [CustomActions._propRISA_INSTALL_TYPE] = _insType,

                // props set by Custom Action
                [CustomActions._propRISA_LICENSE_TYPE] = _dummy,
                [CustomActions._propRISA_PRODUCT_TITLE2_INSTYPE] = _dummy,
                [CustomActions._propRISA_PRODUCT_VERSION2] = _dummy,
                [CustomActions._propRISA_PRODUCT_VERSION34] = _dummy,
                [CustomActions._propRISA_STATUS_CODE] = _dummy,
                [CustomActions._propRISA_STATUS_TEXT] = _dummy,
                [CustomActions._propRISA_UPDATE_DATA_VALUE] = _dummy,
                [CustomActions._propRISA_USERFILES] = _dummy
            };
            _sessDTO_dummyValue_PreInstall_Result_property = SessionDTO.Clone(_sessDTO_undefined_PreInstall_Result_property);
            _sessDTO_dummyValue_PreInstall_Result_property[CustomActions._propRISA_SI_PREINSTALL_RESULT] = _dummy;

            _sessDTO_abort_from_PreInstall = SessionDTO.Clone(_sessDTO_undefined_PreInstall_Result_property);
            _sessDTO_abort_from_PreInstall[CustomActions._propRISA_SI_PREINSTALL_RESULT] = CustomActions._sts_SILENT_ERR_REMOVE_INSTALLED_PRODUCT;

            _sessDTO_continue_from_PreInstall = SessionDTO.Clone(_sessDTO_undefined_PreInstall_Result_property);
            _sessDTO_continue_from_PreInstall[CustomActions._propRISA_SI_PREINSTALL_RESULT] = CustomActions._sts_SILENT_OK;

        }
        private SessionDTO _sessDTO_undefined_PreInstall_Result_property;
        private SessionDTO _sessDTO_dummyValue_PreInstall_Result_property;
        private SessionDTO _sessDTO_abort_from_PreInstall;

        private SessionDTO _sessDTO_continue_from_PreInstall;
        private const string _risa3D = "RISA-3D";
        private const string _risa3Dversion = "19.0.0";
        private const string _insType = "Standalone";
        private const string _dummy = "Dummy";

        [TestMethod, TestCategory("SiInitProps")]
        public void CmdLine_Loud_OK()
        {
            var cmdLine = $@"install_3d_1900.exe";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine
            };
            var siRes = CustomActions.silentInitProperties(_sessDTO_continue_from_PreInstall, btd);

            expecting(!siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsOK);
        }

        #region Handling Result from Si_PreInstall, defined or not

        // all tests after these define Si-PreInstall issued an OK to continue

        [TestMethod, TestCategory("SiInitProps")]
        public void Silent_PreInstall_Result_PropNotDefined_Fail()
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
            var siRes = CustomActions.silentInitProperties(_sessDTO_undefined_PreInstall_Result_property, btd);
            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsFail);
        }

        [TestMethod, TestCategory("SiInitProps")]
        public void Silent_PreInstall_Result_PropNotSet_Fail()
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
            var siRes = CustomActions.silentInitProperties(_sessDTO_dummyValue_PreInstall_Result_property, btd);
            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsFail);
        }

        [TestMethod, TestCategory("SiInitProps")]
        public void Silent_PreInstall_Abort_Fail()
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
            var siRes = CustomActions.silentInitProperties(_sessDTO_abort_from_PreInstall, btd);
            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsFail);
        }

        #endregion

        #region Logging

        [TestMethod, TestCategory("SiInitProps")]
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
            var siRes = CustomActions.silentInitProperties(_sessDTO_continue_from_PreInstall, btd);

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsOK);
        }

        [TestMethod, TestCategory("SiInitProps")]
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
            var siRes = CustomActions.silentInitProperties(_sessDTO_continue_from_PreInstall, btd);

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsOK);
        }

        [TestMethod, TestCategory("SiInitProps")]
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
            var siRes = CustomActions.silentInitProperties(_sessDTO_continue_from_PreInstall, btd);

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsOK);
        }

        #endregion

        #region Install Dir

        [TestMethod, TestCategory("SiInitProps")]
        public void InsDir_Normal_OK()
        {
            var propStr = propKvP(_propLogFile, "DESKTOP");
            var propInsDir = propKvP(_propInsDir, @"C:\Bob");
            var cmdLine = $@"{exeShortFn} /qn {propStr} {propInsDir}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.silentInitProperties(_sessDTO_continue_from_PreInstall, btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsOK);
        }

        [TestMethod, TestCategory("SiInitProps")]
        public void InsDir_OneDrive_LookAt_Log()
        {
            var propStr = propKvP(_propLogFile, "DESKTOP");
            var propInsDir = propKvP(_propInsDir, @"C:\Users\Kirk\OneDrive");
            var cmdLine = $@"{exeShortFn} /qn {propStr} {propInsDir}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.silentInitProperties(_sessDTO_continue_from_PreInstall, btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsOK);
        }

        [TestMethod, TestCategory("SiInitProps")]
        public void InsDir_Roaming_LookAt_Log()
        {
            var propStr = propKvP(_propLogFile, "DESKTOP");
            var propInsDir = propKvP(_propInsDir, @"C:\Users\Kirk\OneDrive");
            var cmdLine = $@"{exeShortFn} /qn {propStr} {propInsDir}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.silentInitProperties(_sessDTO_continue_from_PreInstall, btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsOK);
        }

        [TestMethod, TestCategory("SiInitProps")]
        public void InsDir_ProgramFiles_LookAt_Log()
        {
            var propStr = propKvP(_propLogFile, "DESKTOP");
            var propInsDir = propKvP(_propInsDir, @"C:\Program Files\Bob");
            var cmdLine = $@"{exeShortFn} /qn {propStr} {propInsDir}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.silentInitProperties(_sessDTO_continue_from_PreInstall, btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent);
            Assert.IsTrue(siRes.IsOK);
        }

        #endregion

        #region License Type

        [TestMethod, TestCategory("SiInitProps")]
        public void LicenseType_Key_OK()
        {
            var propStr = propKvP(_propLogFile, "DESKTOP");
            var propLicType = propKvP(_propLicType, BootstrapperDataCommon._ltKey);
            var cmdLine = $@"{exeShortFn} /qn {propStr} {propLicType}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.silentInitProperties(_sessDTO_continue_from_PreInstall, btd, new ConsoleLog());

            expecting(_sessDTO_continue_from_PreInstall[CustomActions._propRISA_LICENSE_TYPE] == BootstrapperDataCommon._ltKey,loc());
            Assert.IsTrue(siRes.IsOK);
        }

        [TestMethod, TestCategory("SiInitProps")]
        public void LicenseType_Network_OK()
        {
            var propStr = propKvP(_propLogFile, "DESKTOP");
            var propLicType = propKvP(_propLicType, BootstrapperDataCommon._ltNetwork);
            var cmdLine = $@"{exeShortFn} /qn {propStr} {propLicType}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.silentInitProperties(_sessDTO_continue_from_PreInstall, btd, new ConsoleLog());

            expecting(_sessDTO_continue_from_PreInstall[CustomActions._propRISA_LICENSE_TYPE] == BootstrapperDataCommon._ltNetwork, loc());
            Assert.IsTrue(siRes.IsOK);
        }

        [TestMethod, TestCategory("SiInitProps")]
        public void LicenseType_Subscription_OK()
        {
            var propStr = propKvP(_propLogFile, "DESKTOP");
            var propLicType = propKvP(_propLicType, BootstrapperDataCommon._ltCloud);
            var cmdLine = $@"{exeShortFn} /qn {propStr} {propLicType}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.silentInitProperties(_sessDTO_continue_from_PreInstall, btd, new ConsoleLog());

            expecting(_sessDTO_continue_from_PreInstall[CustomActions._propRISA_LICENSE_TYPE] == BootstrapperDataCommon._ltCloud, loc());
            Assert.IsTrue(siRes.IsOK);
        }

        #endregion

        #region Region

        [TestMethod, TestCategory("SiInitProps")]
        public void Region_Canada_OK()
        {
            var propStr = propKvP(_propLogFile, "DESKTOP");
            var propRgn = propKvP(_propRegion, "1");
            var cmdLine = $@"{exeShortFn} /qn {propStr} {propRgn}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.silentInitProperties(_sessDTO_continue_from_PreInstall, btd, new ConsoleLog());

            expecting(_sessDTO_continue_from_PreInstall[CustomActions._propRISA_REGION_NAME] == CustomActions._regionCANADA, loc());
            Assert.IsTrue(siRes.IsOK);
        }


        #endregion

        #region Auto Check for Updates

        [TestMethod, TestCategory("SiInitProps")]
        public void AutoCheckUpdates_Yes_OK()
        {
            var propStr = propKvP(_propLogFile, "DESKTOP");
            var propUpd = propKvP(_propUpdate, BootstrapperDataCommon._ansYes);
            var cmdLine = $@"{exeShortFn} /qn {propStr} {propUpd}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.silentInitProperties(_sessDTO_continue_from_PreInstall, btd, new ConsoleLog());

            expecting(_sessDTO_continue_from_PreInstall[CustomActions._propRISA_CHECK_UPDATES] == CustomActions._boolTrue, loc());
            Assert.IsTrue(siRes.IsOK);
        }

        [TestMethod, TestCategory("SiInitProps")]
        public void AutoCheckUpdates_No_OK()
        {
            var propStr = propKvP(_propLogFile, "DESKTOP");
            var propUpd = propKvP(_propUpdate, BootstrapperDataCommon._ansNo);
            var cmdLine = $@"{exeShortFn} /qn {propStr} {propUpd}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.silentInitProperties(_sessDTO_continue_from_PreInstall, btd, new ConsoleLog());

            expecting(_sessDTO_continue_from_PreInstall[CustomActions._propRISA_CHECK_UPDATES] == CustomActions._boolFalse, loc());
            Assert.IsTrue(siRes.IsOK);
        }


        #endregion
    }
}
