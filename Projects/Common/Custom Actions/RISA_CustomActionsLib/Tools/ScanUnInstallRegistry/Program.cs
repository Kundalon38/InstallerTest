using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace ScanUnInstallRegistry
{
    class Program
    {
        static void Main(string[] args)
        {
            //if (args.Length < 2) return;
            ////
            //// arg[0] = product name to search for
            //// arg[1] = filename to write the result to
            ////
            //var productNameToSearchFor = args[0];
            //var outFn = args[1];
            //
            // DEBUG commented out - hardwired test inputs
            var deskTop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var outFn = Path.Combine(deskTop, "Installed.txt");
            // end DEBUG commented out
            //

            var productNameToSearchFor = "RISA-3D";     // DEBUG - happens to be installed with IA
            List<string> outList;
            using (var ins = new InstallAwareRegistry())
            {
                outList = searchForProductInRegistryHive(ins, productNameToSearchFor);
            }

            productNameToSearchFor = "RISAConnection";     // DEBUG - happens to be installed with AI
            using (var ins = new AdvancedInstallerRegistry())
            {
                outList.AddRange(searchForProductInRegistryHive(ins, productNameToSearchFor));
            }

            if (outList.Count == 0) return;
            try
            {
                using (var sw = new StreamWriter(outFn))
                {
                    foreach(var s in outList) sw.WriteLine(s);
                }
            }
            catch (Exception e)
            {
            }
        }

        private static List<string> searchForProductInRegistryHive(IInstallerVendorRegistry vendor, string productNameToSearchFor)
        {
            var outList = new List<string>();
            var unSubKeys = vendor.UnInstallHiveKey.GetSubKeyNames();
            const string at = "@";

            //display(unSubKeys.Length + " UnInstall subkeys found");
            foreach (var unSubKey in unSubKeys)
            {
                var productKey = vendor.UnInstallHiveKey.OpenSubKey(unSubKey);
                var dispName = productKey.GetValue("DisplayName");                 // will be RISA-3D 19.0
                if (dispName != null)
                {
                    var dispNameStr = dispName.ToString();
                    if (dispNameStr.StartsWith(productNameToSearchFor))
                    {
                        var dispVersionObj = productKey.GetValue("DisplayVersion");    // will be 19.0.0.5368
                        if (dispVersionObj != null)
                        {
                            var productInstallLocObj = productKey.GetValue("InstallLocation");
                            if (productInstallLocObj != null)
                            {
                                var installerLocObj = productKey.GetValue("ModifyPath");
                                if (installerLocObj != null)
                                {
                                    var productName = dispNameStr.Split(' ')[0];      // will be RISA-3D
                                    var dispVersion = dispVersionObj.ToString();
                                    var productInstallLoc = productInstallLocObj.ToString();
                                    var installerLoc = installerLocObj.ToString();

                                    var outStr = $"{vendor.VendorCode}{at}{productName}{at}{dispVersion}{at}{productInstallLoc}{at}{installerLoc}";
                                    outList.Add(outStr);
                                }
                            }
                        }
                    }
                }
                productKey.Close();
            }
            vendor.UnInstallHiveKey.Close();
            return outList;
        }
    }

    interface IInstallerVendorRegistry
    {
        string VendorCode { get; }
        RegistryKey RegViewKey { get; }
        RegistryKey UnInstallHiveKey { get; }
        string RegistryPath { get; }
    }

    class AdvancedInstallerRegistry : IInstallerVendorRegistry, IDisposable
    {
        public string VendorCode { get; }
        public RegistryKey RegViewKey { get; }
        public RegistryKey UnInstallHiveKey { get; }
        public string RegistryPath { get; }

        public AdvancedInstallerRegistry()
        {
            VendorCode = "AI";
            RegViewKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            RegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";              // AI installations
            UnInstallHiveKey = RegViewKey.OpenSubKey(RegistryPath);
        }

        public void Dispose()
        {
            UnInstallHiveKey.Close();
            RegViewKey.Close();
        }
    }

    class InstallAwareRegistry : IInstallerVendorRegistry, IDisposable
    {
        public string VendorCode { get; }
        public RegistryKey RegViewKey { get; }
        public RegistryKey UnInstallHiveKey { get; }
        public string RegistryPath { get; }

        public InstallAwareRegistry()
        {
            VendorCode = "IA";
            RegViewKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            RegistryPath = @"SOFTWARE\wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";   // IA installations
            UnInstallHiveKey = RegViewKey.OpenSubKey(RegistryPath);
        }

        public void Dispose()
        {
            UnInstallHiveKey.Close();
            RegViewKey.Close();
        }
    }
}
