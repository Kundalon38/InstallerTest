using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RISA_CustomActionsLib.Models;
using BootstrapperData = RISA_CustomActionsLib.Models.Linked.BootstrapperData;

namespace RISA_CustomActionsLib.Test
{
    [TestClass]
    public class BootstrapperData_Tests:TestBase
    {
        public const string dq = @"""";
        public const string myProp = "MYPROP";
        public const string myPropValue = "MY VALUE";
        public const string myProp2 = "MYPROP2";
        public const string myProp2Value = "MY VALUE2";

        // naming: exe_slashQ_propValuePair

        [TestMethod]
        public void TestCmd_exe_slashQ_propValuePair_OK()
        {
            var cmdLine = $@"install_3d_1900.exe /qn {myProp}={dq}{myPropValue}{dq}";
            var bd = new BootstrapperData(cmdLine);
            expecting(bd.IsSilent);
            bd.ParseCmdLine();
            expecting(bd.CmdLineProperties.Count == 1);
            expecting(bd.CmdLineProperties[0].PropName == myProp);
            expecting(bd.CmdLineProperties[0].PropValue == myPropValue);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestCmd_exeSlashQ_propValuePair_OK()
        {
            var cmdLine = $@"install_3d_1900.exe/qn {myProp}={dq}{myPropValue}{dq}";
            var bd = new BootstrapperData(cmdLine);
            expecting(bd.IsSilent);
            bd.ParseCmdLine();
            expecting(bd.CmdLineProperties.Count == 1);
            expecting(bd.CmdLineProperties[0].PropName == myProp);
            expecting(bd.CmdLineProperties[0].PropValue == myPropValue);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestCmd_exe_NotSilent_OK()
        {
            var cmdLine = $@"install_3d_1900.exe {myProp}={dq}{myPropValue}{dq}";
            var bd = new BootstrapperData(cmdLine);
            expecting(!bd.IsSilent);
            expecting(bd.CmdLineProperties.Count == 0);
            Assert.IsTrue(true);
        }
        [TestMethod]
        public void TestCmd_exeSlashQ_propValuePair2_OK()
        {
            var cmdLine = $@"install_3d_1900.exe/qn {myProp}={dq}{myPropValue}{dq} {myProp2}={dq}{myProp2Value}{dq}";
            var bd = new BootstrapperData(cmdLine);
            expecting(bd.IsSilent);
            bd.ParseCmdLine();
            expecting(bd.CmdLineProperties.Count == 2);
            expecting(bd.CmdLineProperties[0].PropName == myProp);
            expecting(bd.CmdLineProperties[0].PropValue == myPropValue);
            expecting(bd.CmdLineProperties[1].PropName == myProp2);
            expecting(bd.CmdLineProperties[1].PropValue == myProp2Value);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestCmd_exeSlashQ_propValuePair_NoDq_Bad()
        {
            var cmdLine = $@"install_3d_1900.exe/qn {myProp}={myPropValue}";
            var bd = new BootstrapperData(cmdLine);
            expecting(bd.IsSilent);
            bd.ParseCmdLine();
            expecting(bd.CmdLineProperties.Count == 0);
            expecting(bd.ErrorList.Count > 0);
            Console.WriteLine(bd.ErrorList[0].Text);
            Assert.IsTrue(true);
        }
        [TestMethod]
        public void TestCmd_exeSlashQ_propValuePair_NoEq_Bad()
        {
            var cmdLine = $@"install_3d_1900.exe/qn {myProp} {myPropValue}";
            var bd = new BootstrapperData(cmdLine);
            expecting(bd.IsSilent);
            bd.ParseCmdLine();
            expecting(bd.CmdLineProperties.Count == 0);
            expecting(bd.ErrorList.Count > 0);
            Console.WriteLine(bd.ErrorList[0].Text);
            Assert.IsTrue(true);
        }
        [TestMethod]
        public void TestCmd_exeSlashQ_propValuePair2_MissingTerminatorBad()
        {
            var cmdLine = $@"install_3d_1900.exe/qn {myProp}={dq}{myPropValue}{dq} {myProp2}={dq}{myProp2Value}";
            var bd = new BootstrapperData(cmdLine);
            expecting(bd.IsSilent);
            bd.ParseCmdLine();
            expecting(bd.CmdLineProperties.Count == 1);
            expecting(bd.CmdLineProperties[0].PropName == myProp);
            expecting(bd.CmdLineProperties[0].PropValue == myPropValue);
            expecting(bd.ErrorList.Count > 0);
            Console.WriteLine(bd.ErrorList[0].Text);
            Assert.IsTrue(true);
        }


    }
}
