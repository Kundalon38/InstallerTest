using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace RISA_CustomActionsLib.Models.Linked
{
    public class InstalledProductList : List<InstalledProduct>
    {
        public static InstalledProductList FindInstalledProducts(string productName)
        {
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


        #region De/Ser

        public string Serialize()
        {
            var serializer = new XmlSerializer(typeof(InstalledProductList));
            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, this, ns);
                return stream.ToString();
            }
        }

        public static InstalledProductList Deserialize(string xmlStr)
        {
            var deserializer = new XmlSerializer(typeof(InstalledProductList));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlStr));
            object o = deserializer.Deserialize(ms);
            return (InstalledProductList)o;
        }

        #endregion
    }
}
