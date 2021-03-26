using System;
using System.Collections.Generic;
using Microsoft.Win32;
using RISA_CustomActionsLib.Extensions;
using RISA_CustomActionsLib.Models;

namespace RISA_CustomActionsLib
{
    public partial class CustomActions
    {
        public static bool serializeMatchingInstalledProducts(SessionDTO sessDTO)
        {
            var insProdList = findInstalledProducts(sessDTO);
            //
            // ensure that user is not installing old product when newer one is installed
            // this version exactly matches the INSTALL_TYPE, for better or worse matching InstallAware implementation
            // - this is regardless of TARGETDIR
            //
            foreach (var installed in insProdList)
            {
                if (installed.ProductName != sessDTO[_propMSI_ProductName]) continue;
                if (installed.InStallType != sessDTO[_propRISA_INSTALL_TYPE]) continue;
                if (installed.ProductVersion.CompareTo(sessDTO[_propMSI_ProductVersion].ToVersion()) <= 0) continue;
                {
                    // installed Version is newer than incoming Version - the only possible user error at this point
                    if (installed.ProductVersion.CompareTo(sessDTO[_propMSI_ProductVersion].ToVersion()) <= 0) continue;

                    sessDTO[_propRISA_STATUS_CODE] = _sts_ERR_INSTALL_OLD_VERSION;
                    sessDTO[_propRISA_STATUS_TEXT] = $"Installing older version of {installed.ProductName}" +
                                                     $"{displayedInstallType(installed.InStallType)}" +
                                                     $" ({sessDTO[_propMSI_ProductVersion]})" +
                                                     $" when newer version {installed.ProductVersion} is already installed";
                    return false;
                }
            }
            sessDTO[_propRISA_INSTALLED_PRODUCTS] = insProdList.Count == 0
                ? string.Empty
                : insProdList.Serialize();
            return true;
        }

        public static InstalledProductList findInstalledProducts(SessionDTO sessDTO)
        {
            var productName = sessDTO[_propMSI_ProductName];
            var insProdList = new InstalledProductList();

            using (var insVendorReg = new InstallAwareRegistry())
            {
                insProdList.AddRange(searchUnInsRegistry(insVendorReg, productName));
            }

            using (var insVendorReg = new AdvancedInstallerRegistry())
            {
                insProdList.AddRange(searchUnInsRegistry(insVendorReg, productName));
            }
            return insProdList;
        }

        private static List<InstalledProduct> searchUnInsRegistry(InstallerVendorRegistry vendorInfo, string productNameToSearchFor)
        {
            var outList = new List<InstalledProduct>();
            var unSubKeys = vendorInfo.UnInstallHiveKey.GetSubKeyNames();

            foreach (var unSubKey in unSubKeys)
            {
                var productKey = vendorInfo.UnInstallHiveKey.OpenSubKey(unSubKey);
                var installedProduct = examineRegistryEntry(vendorInfo, productNameToSearchFor, productKey);
                if (installedProduct != null) outList.Add(installedProduct);
                productKey?.Close();
            }
            vendorInfo.UnInstallHiveKey.Close();
            return outList;
        }

        private static InstalledProduct examineRegistryEntry(InstallerVendorRegistry vendorInfo,
            string productNameToSearchFor, RegistryKey productKey)
        {
            var dispName = productKey.GetValue("DisplayName"); // DisplayNames are:  'RISA-3D 19.0' or 'RISA-3D 19.0 Demo'
            if (dispName == null) return null;

            var dispNameStr = dispName.ToString();
            if (!dispNameStr.StartsWith(productNameToSearchFor)) return null;

            var dispVersionObj = productKey.GetValue("DisplayVersion"); // a 4 part version, eg 19.0.0.5368
            if (dispVersionObj == null) return null;

            var productInstallLocObj = productKey.GetValue("InstallLocation");
            if (productInstallLocObj == null) return null;

            var installerLocObj = productKey.GetValue("ModifyPath");
            if (installerLocObj == null) return null;

            var dispVersion = dispVersionObj.ToString();
            var productInstallLoc = productInstallLocObj.ToString();
            var installerLoc = installerLocObj.ToString();

            return new InstalledProduct(vendorInfo.Vendor, dispNameStr, dispVersion, productInstallLoc,
                installerLoc);
        }
    }
}
