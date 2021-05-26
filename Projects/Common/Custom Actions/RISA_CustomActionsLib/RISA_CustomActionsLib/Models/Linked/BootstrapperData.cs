using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RISA_CustomActionsLib.Models.Linked
{
    // this class is Linked by the Silent-PreInstall project outside RISA_CustomActionsLib
    //  only one copy of source code, DRY.


    public partial class BootstrapperData
    {
        public BootstrapperData()
        {
            // exists to collect errors - which may be logged - before cmdLine is found
        }

        public BootstrapperData(string cmdLine, List<SiError> previousErrs = null)
        {
            if (previousErrs != null) ErrorList = previousErrs;
            if (cmdLine == null) return;

            CmdLine = cmdLine;
            var cmdLineUC = cmdLine.ToUpper();
            IsSilent = cmdLineUC.Contains(@"/Q");
        }

        public string CmdLine { get; }
        public bool IsSilent { get; }

        #region Logging

        public string LogFileName
        {
            // null means no logging was requested
            get
            {
                var logKvp = CmdLineProperties.FirstOrDefault(x => x.PropName == _propLogFile);
                if (logKvp == null) return null;

                const string deskTop = "DESKTOP";
                if (string.Compare(logKvp.PropValue, deskTop, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    var deskTopDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    return Path.Combine(deskTopDir, shortLogFileName);
                }
                // take the logKvp.PropValue as the absolute path
                return logKvp.PropValue;
            }
        }

        private string shortLogFileName
        {
            get
            {
                const string onEmptyFn = "RISA_Product_Installer.log";
                if (string.IsNullOrEmpty(ExeFullName)) return onEmptyFn;

                var shortExeFn = Path.GetFileName(ExeFullName);
                return Path.ChangeExtension(shortExeFn, ".log");
            }
        }

        public List<SiError> ErrorList { get; set; } = new List<SiError>();

        private void addErr(string text, bool isFatal = true)
        {
            // a little shorter^ than:
            ErrorList.Add(new SiError(text, isFatal));
        }

        #endregion

        #region Bootstrapper Exe and the Product Info it Contains

        public void GetExeData(Process bootstrapperProcess)
        {
            try
            {
                ExeFullName = bootstrapperProcess.GetExecutablePath();
            }
            catch (Exception ex)
            {
                addErr($"{loc()} GetExecutablePath Excp {ex.Message}");
            }
            if (ExeFullName == null) return;
            try
            {
                var fvi = FileVersionInfo.GetVersionInfo(ExeFullName);
                ProductName = fvi.ProductName;
                ProductVersionStr = fvi.ProductVersion;
            }
            catch (Exception ex)
            {
                addErr($"{loc()} GetVersionInfo Excp {ex.Message}");
            }

        }

        public string ProductName { get; private set; }
        public string ProductVersionStr { get; private set; }

        public string ExeFullName { get; private set; }

        public bool ValidateProduct()
        {
            if (string.IsNullOrEmpty(ProductName)) 
            {
                addErr("Missing Product Name");
                return false;
            }
            if (_supportedProductNames.Contains(ProductName)) return true;
            addErr($"Unsupported Product Name for Silent Install: {ProductName}");
            return false;
        }

        public bool ValidateVersion()
        {
            if (string.IsNullOrEmpty(ProductVersionStr))
            {
                addErr("Missing Product Version");
                return false;
            }
            try
            {
                var versn = ProductVersionStr.ToVersion();
                return true;
            }
            catch (Exception e)
            {
                addErr($"Invalid (non-numeric) {ProductName} version: {ProductVersionStr}");
                return false;
            }
        }

        private const string _prod2D = "RISA-2D";
        private const string _prod3D = "RISA-3D";
        private const string _prodFD = "RISAFloor";
        private const string _prodFL = "RISAFoundation";
        private const string _prodCN = "RISAConnection";

        private readonly string[] _supportedProductNames = {_prod2D, _prod3D, _prodFD, _prodFL, _prodCN};


        #endregion

        #region Parse CmdLine

        public List<CmdLineProperty> CmdLineProperties { get; } = new List<CmdLineProperty>();

        private enum eParseState
        {
            Initial,
            InitialBlank,
            InPropName,
            PropNameEnd,
            MidExpressionEq,
            InPropValue
        }

        public bool ParseCmdLine()
        {
            if (string.IsNullOrEmpty(CmdLine)) return false;

            // see the grammar diagram in Silent Design.vsdx (visio)
            //
            const byte slash = 0x2F;
            const char blank = ' ';
            const byte dq = 0x22;
            const char eq = '=';

            var state = eParseState.Initial;
            var cmdChars = CmdLine.ToCharArray();
            var propNameStr = string.Empty; // build the property name
            var propValueStr = string.Empty; // build the property value when found

            CmdLineProperties.Clear();
            var parseErr = false;

            for (var iChar = 0; iChar < cmdChars.Length; iChar++)
            {
                var c = cmdChars[iChar];
                switch (state)
                {
                    case eParseState.Initial:
                        if (c != blank) break; // get past 1st token, bootstrapper.exe
                        state = eParseState.InitialBlank;
                        propValueStr = string.Empty;
                        propNameStr = string.Empty;
                        break;

                    case eParseState.InitialBlank:
                        if (c == blank) break;
                        if (propNameStr.Length == 0 && c == slash)
                        {
                            state = eParseState.Initial;
                            break;
                        }

                        state = eParseState.InPropName;
                        propNameStr += c.ToString();
                        break;

                    case eParseState.InPropName:
                        switch (c)
                        {
                            case blank:
                                state = eParseState.PropNameEnd;
                                break;
                            case eq:
                                state = eParseState.MidExpressionEq;
                                break;
                            default:
                                propNameStr += c.ToString();
                                break;
                        }

                        break;

                    case eParseState.PropNameEnd:
                        switch (c)
                        {
                            case blank:
                                break;
                            case eq:
                                state = eParseState.MidExpressionEq;
                                break;
                            default:
                                addErr($"Invalid character in command line at position {iChar}, expecting equals sign");
                                parseErr = true;
                                break;
                        }

                        break;

                    case eParseState.MidExpressionEq:
                        switch (c)
                        {
                            case blank:
                                break;
                            case (char) dq:
                                state = eParseState.InPropValue;
                                propValueStr = string.Empty;
                                break;
                            default:
                                addErr($"Invalid character in command line at position {iChar}, expecting double quote");
                                parseErr = true;
                                break;
                        }

                        break;

                    case eParseState.InPropValue:
                        if (c == dq)
                        {
                            // combine prop name + value
                            CmdLineProperties.Add(new CmdLineProperty(propNameStr, propValueStr));
                            state = eParseState.Initial;
                        }
                        else propValueStr += c.ToString();
                        break;
                }
                if (parseErr) break;
            }
            if (parseErr) return false;

            if (state == eParseState.Initial || state == eParseState.InitialBlank) return true;
            addErr($"Incomplete parse of command line, expression isn't terminated");
            return false;
        }

        private const string _propInsDir = "SIDIR";
        private const string _propPgmGrp = "SIGRP";
        private const string _propRegion = "SIRGN";
        private const string _propUpdate = "SIUPD";
        private const string _propLicType = "SILTY";
        private const string _propLogFile = "SILOG";
        private const string _propIniFile = "SINIF";

        #endregion

        public override string ToString()
        {
            // TODO expand
            var cr = Environment.NewLine;
            return
                $"CmdLine={CmdLine.ToDetailStr()}{cr}IsSilent={IsSilent}{cr}ProductName={ProductName.ToDetailStr()}{cr}ProductVersion={ProductVersionStr}";
        }
        private string loc([System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            // return className.MethodName
            return $"{GetType().Name}.{memberName}";
        }

    }
}
