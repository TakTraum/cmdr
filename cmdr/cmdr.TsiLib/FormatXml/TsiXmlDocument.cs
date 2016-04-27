using cmdr.TsiLib.FormatXml.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using cmdr.TsiLib.FormatXml.XDocumentExtensions;
using System.IO;
using cmdr.TsiLib.FormatXml.Interpretation;

namespace cmdr.TsiLib.FormatXml
{
    public class TsiXmlDocument
    {
        private static readonly string XPATH_ROOT = "/NIXML/TraktorSettings";
        private static readonly string XPATH_ENTRY_TEMPLATE = XPATH_ROOT + "/Entry[@Name='{0}']";

        private readonly XDocument _doc;
        private readonly XElement _root;


        public TsiXmlDocument()
        {
            _doc = new XDocument(new XDeclaration("1.0", "UTF-8", "no"), new XElement("NIXML", new XElement("TraktorSettings")));
            _root = _doc.XPathSelectElement(XPATH_ROOT);

            SaveEntry(new Flavour { Value = -1 });
            SaveEntry(new cmdr.TsiLib.FormatXml.Interpretation.Version { Value = 0 });
        }

        public TsiXmlDocument(string filePath)
        {
            using (StreamReader source = new StreamReader(getFileReadStream(filePath)))
                _doc = XDocument.Load(source);
            _root = _doc.XPathSelectElement(XPATH_ROOT);
        }


        public AXmlEntry GetEntry(string name)
        {
            return AXmlEntry.Parse(getEntry(name));
        }

        public T GetEntry<T>(T entry) where T : AXmlEntry
        {
            var rawEntry = getEntry(entry.Name);
            if (rawEntry != null)
            {
                entry.Load(rawEntry);
                return entry;
            }
            return null;
        }

        public T GetEntry<T>() where T : AXmlEntry, new()
        {
            var entry = new T();
            var rawEntry = getEntry(entry.Name);
            if (rawEntry != null)
            {
                entry.Load(rawEntry);
                return entry;
            }
            return null;
        }

        public void SaveEntry<T>(T entry) where T : AXmlEntry
        {
            entry.Save(_root);
        }

        public void Save(string filePath)
        {
            _root.Sort();

            using (StreamWriter destination = new StreamWriter(getFileWriteStream(filePath)))
                _doc.Save(destination);
        }


        private XElement getEntry(string name)
        {
            return _doc.XPathSelectElement(String.Format(XPATH_ENTRY_TEMPLATE, name));
        }

        private static Stream getFileReadStream(string filePath)
        {
            return File.Open(filePath, FileMode.Open, FileAccess.Read);
        }

        private static Stream getFileWriteStream(string filePath)
        {
            return File.Open(filePath, FileMode.Create, FileAccess.Write);
        }
    }
}
