using System;
using Microsoft.Win32;

namespace RISA_CustomActionsLib.Models.Linked
{
    // This handles InstallAware and AdvancedInstaller
    //
    // Years ago, RISA used InstallShield.  RISA Section was installed this way.
    // If you need to extend this for InstallShield, RISA's InstallAware scripts handle finding + removing
    //  InstallShield-installed products, such as earlier versions of RISA Section (the most recent RISA Section uses InstallAware)
    // Because InstallShield is a fringe case (for very old/obsolete RISA products), it's not implemented here
    // - implementing InstallShield for ADAPT is left to the future
    //
    public enum eInstallerVendor { InstallAware, AdvancedInstaller }

    public abstract class InstallerVendorRegistry: IDisposable
    {
        public eInstallerVendor Vendor { get; }
        public RegistryKey RegViewKey { get; protected set; }
        public RegistryKey UnInstallHiveKey { get; protected set; }
        public string RegistryPath { get; protected set; }

        protected InstallerVendorRegistry(eInstallerVendor vendor)
        {
            Vendor = vendor;
            RegViewKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
        }

        public void Dispose()
        {
            UnInstallHiveKey.Close();
            RegViewKey.Close();
        }
    }

    public class AdvancedInstallerRegistry : InstallerVendorRegistry
    {
        public AdvancedInstallerRegistry() : base(eInstallerVendor.AdvancedInstaller)
        {
            RegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";              // AI installations
            UnInstallHiveKey = RegViewKey.OpenSubKey(RegistryPath);
        }
    }

    public class InstallAwareRegistry : InstallerVendorRegistry
    {
        public InstallAwareRegistry() : base(eInstallerVendor.InstallAware)
        {
            RegistryPath = @"SOFTWARE\wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";   // IA installations
            UnInstallHiveKey = RegViewKey.OpenSubKey(RegistryPath);
        }
    }
}
