using System;
using System.Diagnostics;
using System.Xml.Serialization;
using RISA_CustomActionsLib.Extensions;

namespace RISA_CustomActionsLib.Models
{
    public class InstalledProduct
    {
        public eInstallerVendor Vendor { get; set; }
        public string DisplayName { get; set; }
        public string ProductVersionStr { get; set; }
        public string TargetDir { get; set; }
        public string UnInstallStr { get; set; }

        public InstalledProduct()
        {
            // default ctor and public setters are required for this particular serializer
        }

        public InstalledProduct(eInstallerVendor vendor, string displayName, string productVersion,
            string targetDir, string unInstallStr)
        {
            Vendor = vendor;
            DisplayName = displayName;
            ProductVersionStr = productVersion;
            TargetDir = targetDir;
            UnInstallStr = unInstallStr;
        }

        // DisplayNames are:  'RISA-3D 19.0' or 'RISA-3D 19.0 Demo'
        //  older product versions have a lot more decorators following the major.minor version
        [XmlIgnore] public string InStallType => DisplayName.Contains(CustomActions._insTypeDemo)
                        ? CustomActions._insTypeDemo : CustomActions._insTypeStandalone;
        [XmlIgnore] public string ProductName => DisplayName.Split(' ')[0];
        [XmlIgnore] public Version ProductVersion => ProductVersionStr.ToVersion();

        public override string ToString()
        {
            return $"Vendor = {Vendor}{Environment.NewLine}" 
                   + $"$DisplayName = {DisplayName}{Environment.NewLine}" 
                   + $"$ProductName = {ProductName}{Environment.NewLine}"
                   + $"$ProductVersionStr = {ProductVersionStr}{Environment.NewLine}"
                   + $"TargetDir = {TargetDir}{Environment.NewLine}"
                   + $"UnInstallStr = {UnInstallStr}";
        }

        public void UnInstall()
        {
            Process uninsProcess = null;
            const int milliSecsWaitForUnInstall = 30*1000;

            switch (Vendor)
            {
                case eInstallerVendor.InstallAware:
                    //
                    // typical IA uninstall string:
                    // C:\ProgramData\{334a52d5-d212-485d-8e9b-f4ed60154877}\install_3d_1900.exe
                    //
                    const string cmdArgsIA = @"/S MODIFY=FALSE REMOVE=TRUE UNINSTALL=YES";
                    uninsProcess = Process.Start(UnInstallStr.Dq(), cmdArgsIA);
                    CustomActions.Trace("UnInstall", $"UnInstallStr.Dq()={UnInstallStr.Dq()} args={cmdArgsIA}");

                    break;

                case eInstallerVendor.AdvancedInstaller:
                    //
                    // typical AI uninstall strings (can't find documentation on AI_UNINSTALLER_CTP=1 - secret switch)
                    // - the simpler string works and is what's used:
                    // msiexec.exe /i {F9535452-B913-4080-B4F0-6F56C3029048} AI_UNINSTALLER_CTP=1
                    // MsiExec.exe /I{F9535452-B913-4080-B4F0-6F56C3029048}
                    //
                    // gets transformed to:
                    // msiexec.exe /x {guid} /qn
                    //
                    const string leftBracket = "{";
                    const string rightBracket = "}";
                    const string cmdExeAI = @"MsiExec.exe";

                    var guidStartPos = UnInstallStr.IndexOf(leftBracket);
                    var guidEndPos = UnInstallStr.IndexOf(rightBracket);

                    var productGuid = UnInstallStr.Substring(guidStartPos, guidEndPos - guidStartPos + 1);
                    var cmdArgsAI = $@"/X {productGuid} /qn";
                    uninsProcess = Process.Start(cmdExeAI, cmdArgsAI);
                    break;

            }
            uninsProcess?.WaitForExit(milliSecsWaitForUnInstall);
        }
    }
}
