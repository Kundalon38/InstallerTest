using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RISA_CustomActionsLib.Models.Linked
{
    // this class is Linked by the Silent-PreInstall project outside RISA_CustomActionsLib
    //  only one copy of source code, DRY.


    public partial class BootstrapperData : BootstrapperDataCommon
    {
        public BootstrapperData(BootstrapperTestData testData = null)
        {
            // exists to collect errors - which may be logged - before cmdLine is found
            // if testData is not null, init BootstrapperData with test data
            // 
            if (testData == null) return;
            ctor_common(testData.CmdLine);
            ExeFullName = testData.ExeFullName;
            ProductName = testData.ProductName;
            ProductVersionStr = testData.ProductVersionStr;
        }

        public BootstrapperData(string cmdLine, List<SiError> previousErrs = null)
        {
            if (previousErrs != null) ErrorList = previousErrs;
            ctor_common(cmdLine);
        }

        private void ctor_common(string cmdLine)
        {
            if (cmdLine == null) return;

            CmdLine = cmdLine;
            IsSilent = cmdLine.ToUpper().Contains(@"/Q");
        }

        public string CmdLine { get; private set; }
        public bool IsSilent { get; private set; }

        #region Logging

        public string LogFileName
        {
            // null means no logging was requested
            get
            {
                var logKvp = CmdLineProperties[_propLogFile];
                if (logKvp == null) return null;

                const string deskTop = "DESKTOP";
                if(logKvp.PropValue.IsEqIgnoreCase(deskTop))
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
            if(ProductName.Contains("Demo")) addErr($"Unsupported Demo product for Silent Install: {ProductName}");
            else addErr($"Unsupported product for Silent Install: {ProductName}");
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

        #endregion

        #region Validate Properties

        public bool ValidatePropertyValues()
        {
            // ignore all but SI* cmd line properties - these are stored UpperCase
            var retSts = true;
            var siProps = CmdLineProperties.Where(x => x.PropName.StartsWith("SI")).ToList();

            // process an ini file if it exists

            var iniFileKvp = siProps.SingleOrDefault(x => x.PropName == _propIniFile);

            BootstrapperIniFile bootIni = null;
            if (iniFileKvp == null)
            {
                var setupIniFn = Path.Combine(Path.GetDirectoryName(ExeFullName), "Setup.Ini");
                if(File.Exists(setupIniFn)) bootIni = new BootstrapperIniFile(setupIniFn, ProductName);
            }
            else
            {
                bootIni = new BootstrapperIniFile(iniFileKvp.PropValue, ProductName);
            }

            if (bootIni != null)
            {
                CmdLineProperties = bootIni.CmdLineProperties;
                ErrorList.AddRange(bootIni.ErrorList);
                if (ErrorList.Any(x => x.IsFatal)) return false;
                siProps = CmdLineProperties.Where(x => x.PropName.StartsWith("SI")).ToList();
            }

            foreach (var siProp in siProps)
            {
                var valueAccepted = false;
                switch (siProp.PropName)
                {
                    case _propRegion:
                        if (siProp.PropValue.Length != 1)
                        {
                            addErr($"Invalid Region code in: {siProp.ToString()}");
                            retSts = false;
                            break;
                        }
                        var rgnNdx = _allValidRegions.IndexOf(siProp.PropValue);
                        if (rgnNdx < 0)
                        {
                            addErr($"Invalid Region code in: {siProp.ToString()}");
                            retSts = false;
                            break;
                        }
                        if (ProductName == _prodCN && rgnNdx > 1)
                        {
                            addErr($"Invalid Region code for {_prodCN} in: {siProp.ToString()}");
                            retSts = false;
                            break;
                        }
                        break;

                    case _propUpdate:
                        // normalize the value, so user can disregard letter case, so can code downstream
                        if (siProp.PropValue.IsEqIgnoreCase(_ansYes))
                        {
                            valueAccepted = true;
                            siProp.PropValue = _ansYes;
                        }
                        else if (siProp.PropValue.IsEqIgnoreCase(_ansNo))
                        {
                            valueAccepted = true;
                            siProp.PropValue = _ansNo;
                        }
                        if (!valueAccepted)
                        {
                            addErr($"Invalid property value in: {siProp.ToString()}");
                            retSts = false;
                        }
                        break;

                    case _propLicType:
                        if (siProp.PropValue.IsEqIgnoreCase(_ltCloud))
                        {
                            valueAccepted = true;
                            siProp.PropValue = _ltCloud;
                        }
                        else if (siProp.PropValue.IsEqIgnoreCase(_ltKey))
                        {
                            valueAccepted = true;
                            siProp.PropValue = _ltKey;
                        }
                        else if (siProp.PropValue.IsEqIgnoreCase(_ltNetwork))
                        {
                            valueAccepted = true;
                            siProp.PropValue = _ltNetwork;
                        }
                        if (!valueAccepted)
                        {
                            addErr($"Invalid License Type in: {siProp.ToString()}");
                            retSts = false;
                        }
                        break;

                    default:
                        if (!Array.Exists(_supportedSiPropNames, el => el == siProp.PropName))
                        {
                            addErr($"Invalid Property: {siProp.PropName}");
                            retSts = false;
                        }
                        break;
                }
            }
            return retSts;
        }

        #endregion

        #region Install Old over New

        public bool IsInstallOldOverNew(InstalledProductList insProdList)
        {
            const string insTypeStandalone = "Standalone";
            foreach (var installed in insProdList)
            {
                if (installed.ProductName != ProductName) continue;
                if (installed.InstallType != insTypeStandalone) continue;
                if (installed.ProductVersion.CompareTo(ProductVersionStr.ToVersion()) <= 0) continue;

                var errmsg = $"Installing older version of {installed.ProductName}" +
                             $"{insTypeStandalone} ({ProductVersionStr})" +
                             $" when newer version {installed.ProductVersion} is already installed";

                addErr(errmsg);
                return true;
            }
            return false;
        }

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
