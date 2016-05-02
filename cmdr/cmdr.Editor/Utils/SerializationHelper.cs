using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace cmdr.Editor.Utils
{
    public static class SerializationHelper
    {
        public static T Deserialize<T>(XDocument doc, bool useDataContract = false)
        {
            if (!useDataContract)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

                using (var reader = doc.Root.CreateReader())
                {
                    return (T)xmlSerializer.Deserialize(reader);
                }
            }
            else
            {
                DataContractSerializer xmlSerializer = new DataContractSerializer(typeof(T));

                using (var reader = doc.Root.CreateReader())
                {
                    return (T)xmlSerializer.ReadObject(reader);
                }
            }
        }

        public static XDocument Serialize<T>(T value, bool useDataContract = false)
        {
            XDocument doc = new XDocument(new XDeclaration("1.0", "UTF-8", "no"));
            if (!useDataContract)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                using (var writer = doc.CreateWriter())
                    xmlSerializer.Serialize(writer, value);
            }
            else
            {
                DataContractSerializer xmlSerializer = new DataContractSerializer(typeof(T));

                using (var writer = doc.CreateWriter())
                    xmlSerializer.WriteObject(writer, value);
            }

            foreach (XElement XE in doc.Root.DescendantsAndSelf())
            {
                // Stripping the namespace by setting the name of the element to it's localname only
                XE.Name = XE.Name.LocalName;
                // replacing all attributes with attributes that are not namespaces and their names are set to only the localname
                XE.ReplaceAttributes((from xattrib in XE.Attributes().Where(xa => !xa.IsNamespaceDeclaration) select new XAttribute(xattrib.Name.LocalName, xattrib.Value)));
            }

            return doc;
        }
    }
}
