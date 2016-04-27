using System;
using System.Xml.Linq;
using System.Xml.XPath;

namespace cmdr.TsiLib.FormatXml.Base
{
    public abstract class AValueXmlEntry<T> : AXmlEntry
    {
        public T Value { get; set; }


        public AValueXmlEntry(string name, TsiXmlEntryType type)
            : base(name, type)
        {

        }

        internal AValueXmlEntry(XElement rawEntry)
            : base(rawEntry)
        {
            Load(rawEntry);
        }


        internal override void Load(XElement rawEntry)
        {
            Value = Decode(rawEntry.Attribute("Value").Value);
        }

        internal override void Save(XElement rawParent)
        {
            XElement entry = rawParent.XPathSelectElement(String.Format("Entry[@Name='{0}']", Name));
            if (entry != null)
                entry.Attribute("Value").Value = Encode(Value);
            else
            {
                entry = new XElement("Entry",
                   new XAttribute("Name", Name),
                   new XAttribute("Type", (int)Type),
                   new XAttribute("Value", Encode(Value)));
                entry.Value = String.Empty;
                rawParent.Add(entry);
            }
        }


        protected abstract T Decode(string value);
        protected abstract string Encode(T value);
    }
}
