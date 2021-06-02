using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RISA_CustomActionsLib.Models.Linked
{
    public class InstalledProductList : List<InstalledProduct>
    {
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
    }
}
