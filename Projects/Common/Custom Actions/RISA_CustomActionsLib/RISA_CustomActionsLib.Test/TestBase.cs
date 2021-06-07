using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RISA_CustomActionsLib.Test
{
    public class TestBase
    {
        internal static string TestDataSourceDir { get; }
        private const string _testDataSourceDir = "TestDataSource";
        //internal static string TestDataSandboxDir { get; private set; }
        //private const string _testDataSandboxDir = "TestDataSandbox";

        static TestBase()
        {
            TestDataSourceDir = findDirectory(_testDataSourceDir);
            //TestDataSandboxDir = findDirectory(_testDataSandboxDir);
        }

        private static string findDirectory(string leafDirPart)
        {
            const char bash = (char)92;
            var exeDir = AppDomain.CurrentDomain.BaseDirectory;
            while (!Directory.Exists(exeDir + bash + leafDirPart))
            {
                var lastBashPos = exeDir.LastIndexOf(bash);
                if (lastBashPos < 0)
                    throw new ApplicationException($"Can't find '{leafDirPart}'");
                exeDir = exeDir.Substring(0, lastBashPos);
            }
            return Path.Combine(exeDir, leafDirPart);
        }

        protected void expecting(bool cond)
        {
            if (cond) return;
            Assert.IsTrue(cond); // will fail
        }
        protected void expecting(bool cond, string loc)
        {
            if (cond) return;
            Assert.IsTrue(cond, $"fails at {loc} ");
        }
        protected string loc([System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            return $"line {sourceLineNumber}";
        }

        #region Consts and Helpers

        protected const string _prod2D = "RISA-2D";
        protected const string _prod3D = "RISA-3D";
        protected const string _prodFD = "RISAFloor";
        protected const string _prodFL = "RISAFoundation";
        protected const string _prodCN = "RISAConnection";

        protected const string _propInsDir = "SIDIR";
        protected const string _propPgmGrp = "SIGRP";
        protected const string _propRegion = "SIRGN";
        protected const string _propUpdate = "SIUPD";
        protected const string _propLicType = "SILTY";
        protected const string _propLogFile = "SILOG";
        protected const string _propIniFile = "SINIF";

        protected const string _ltCloud = "Subscription";
        protected const string _ltNetwork = "Network";
        protected const string _ltKey = "Key";
        protected const string _ansYes = "Yes";
        protected const string _ansNo = "No";

        protected string propKvP(string propName, string propValue)
        {
            const string dq = @"""";
            return $"{propName}={dq}{propValue}{dq}";
        }

        protected const string exeShortFn = "installer_3d_1903.exe";

        protected string exeFullFn => Path.Combine(deskTopDir, exeShortFn);

        protected string deskTopDir => Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

        #endregion
    }
}
