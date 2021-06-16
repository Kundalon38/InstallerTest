using RISA_CustomActionsLib.Models;
using RISA_CustomActionsLib.Models.Linked;

namespace RISA_CustomActionsLib
{
    public partial class CustomActions
    {
        public static bool serializeMatchingInstalledProducts(SessionDTO sessDTO)
        {
            var insProdList = InstalledProductList.FindInstalledProducts(sessDTO[_propMSI_ProductName]);
            //
            // return F if incoming product is older than newer one already installed
            // return T otherwise, plus the serialized installed product list in _propRISA_INSTALLED_PRODUCTS
            //
            // this version exactly matches the INSTALL_TYPE, for better or worse matching InstallAware implementation
            // - this is regardless of TARGETDIR
            //
            foreach (var installed in insProdList)
            {
                if (installed.ProductName != sessDTO[_propMSI_ProductName]) continue;
                if (installed.InstallType != sessDTO[_propRISA_INSTALL_TYPE]) continue; // TODO if-condition repeated 2x
                if (installed.ProductVersion.CompareTo(sessDTO[_propMSI_ProductVersion].ToVersion()) <= 0) continue;
                {
                    // installed Version is newer than incoming Version - the only possible user error at this point
                    if (installed.ProductVersion.CompareTo(sessDTO[_propMSI_ProductVersion].ToVersion()) <= 0) continue;

                    sessDTO[_propRISA_STATUS_CODE] = _sts_ERR_INSTALL_OLD_VERSION;
                    sessDTO[_propRISA_STATUS_TEXT] = $"Installing older version of {installed.ProductName}" +
                                                     $"{displayedInstallType(installed.InstallType)}" +
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

    }
}
