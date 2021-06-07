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
    public class SiValidate_INIfile_Tests : TestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            _desktopSetupIniFn = Path.Combine(deskTopDir, _setupIniShortFn);
            try
            {
                File.Delete(_desktopSetupIniFn); // if anything was there
            }
            catch (Exception e)
            {
            }
        }

        private const string _setupIniShortFn = "Setup.Ini";
        private const string _testIni_FromDocsFn = "Setup_FromDocs.ini";
        private const string _testIni_Missing3DFn = "Setup_Missing_RISA-3D.ini";
        private const string _testIni_BadPropNameFn = "Setup_BadPropName.ini";
        private const string _testIni_BadPropValueFn = "Setup_BadPropValue.ini";
        private string _desktopSetupIniFn;

        [TestMethod, TestCategory("SiVal_INIfile")]
        public void Cant_Find_IniFile_Fail()
        {
            var propStr = propKvP(_propIniFile, _desktopSetupIniFn);
            var cmdLine = $@"{exeShortFn} /qn {propStr}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent, loc());
            expecting(siRes.IsFail, loc());
            Assert.IsTrue(true);
        }
        [TestMethod, TestCategory("SiVal_INIfile")]
        public void Can_Find_UnSpecified_IniFile_OK()
        {
            var srcFn = Path.Combine(TestDataSourceDir, _testIni_FromDocsFn);
            File.Copy(srcFn, _desktopSetupIniFn);

            var cmdLine = $@"{exeShortFn} /qn";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent, loc());
            expecting(siRes.BooData.CmdLineProperties.Count > 0, loc());
            expecting(siRes.IsOK, loc());
            Assert.IsTrue(true);
        }

        [TestMethod, TestCategory("SiVal_INIfile")]
        public void Can_Find_Specified_IniFile_OK()
        {
            var srcFn = Path.Combine(TestDataSourceDir, _testIni_FromDocsFn);
            File.Copy(srcFn, _desktopSetupIniFn);

            var propStr = propKvP(_propIniFile, _desktopSetupIniFn);
            var cmdLine = $@"{exeShortFn} /qn {propStr}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent, loc());
            expecting(siRes.BooData.CmdLineProperties.Count > 0, loc());
            expecting(siRes.IsOK, loc());
            Assert.IsTrue(true);
        }

        [TestMethod, TestCategory("SiVal_INIfile")]
        public void Ini_Missing_ProductName_Section_Fail()
        {
            var srcFn = Path.Combine(TestDataSourceDir, _testIni_Missing3DFn);
            File.Copy(srcFn, _desktopSetupIniFn);

            var propStr = propKvP(_propIniFile, _desktopSetupIniFn);
            var cmdLine = $@"{exeShortFn} /qn {propStr}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent, loc());
            //expecting(siRes.BooData.CmdLineProperties.Count > 0, loc());
            expecting(siRes.IsFail, loc());
            Assert.IsTrue(true);
        }
        [TestMethod, TestCategory("SiVal_INIfile")]
        public void Ini_Bad_PropertyName_Fail()
        {
            var srcFn = Path.Combine(TestDataSourceDir, _testIni_BadPropNameFn);
            File.Copy(srcFn, _desktopSetupIniFn);

            var propStr = propKvP(_propIniFile, _desktopSetupIniFn);
            var cmdLine = $@"{exeShortFn} /qn {propStr}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent, loc());
            //expecting(siRes.BooData.CmdLineProperties.Count > 0, loc());
            expecting(siRes.IsFail, loc());
            Assert.IsTrue(true);
        }

        [TestMethod, TestCategory("SiVal_INIfile")]
        public void Ini_Props_and_Values_MappedTo_SiProps_OK()
        {
            var srcFn = Path.Combine(TestDataSourceDir, _testIni_FromDocsFn);
            File.Copy(srcFn, _desktopSetupIniFn);

            var propStr = propKvP(_propIniFile, _desktopSetupIniFn);
            var cmdLine = $@"{exeShortFn} /qn {propStr}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent, loc());

            var siPgmGrpProp = siRes.BooData.CmdLineProperties.SingleOrDefault(x => x.PropName == _propPgmGrp);
            expecting(siPgmGrpProp != null, loc());
            expecting(siPgmGrpProp.PropValue == "RISA", loc());

            var siRegionProp = siRes.BooData.CmdLineProperties.SingleOrDefault(x => x.PropName == _propRegion);
            expecting(siRegionProp != null, loc());
            expecting(siRegionProp.PropValue == "0", loc());

            var siUpdateProp = siRes.BooData.CmdLineProperties.SingleOrDefault(x => x.PropName == _propUpdate);
            expecting(siUpdateProp != null, loc());
            expecting(siUpdateProp.PropValue == _ansYes, loc());

            var siLicTypeProp = siRes.BooData.CmdLineProperties.SingleOrDefault(x => x.PropName == _propLicType);
            expecting(siLicTypeProp != null, loc());
            expecting(siLicTypeProp.PropValue == _ltCloud, loc());

            expecting(siRes.IsOK, loc());
            Assert.IsTrue(true);
        }

        [TestMethod, TestCategory("SiVal_INIfile")]
        public void Ini_Props_and_Values_MappedTo_SiProps_BadValue_Fail()
        {
            var srcFn = Path.Combine(TestDataSourceDir, _testIni_BadPropValueFn);
            File.Copy(srcFn, _desktopSetupIniFn);

            var propStr = propKvP(_propIniFile, _desktopSetupIniFn);
            var cmdLine = $@"{exeShortFn} /qn {propStr}";
            var btd = new BootstrapperTestData()
            {
                CmdLine = cmdLine,
                ExeFullName = exeFullFn,
                ProductName = _prod3D,
                ProductVersionStr = "19.0.3"
            };
            var siRes = CustomActions.SilentValidate(btd, new ConsoleLog());

            expecting(siRes.BooData.IsSilent, loc());
            //expecting(siRes.BooData.CmdLineProperties.Count > 0, loc());
            expecting(siRes.IsFail, loc());
            Assert.IsTrue(true);
        }
    }
}
