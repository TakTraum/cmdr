using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace cmdr.TsiLib.FormatXml.Base
{
    public abstract class AXmlEntry
    {
        public string Name { get; private set; }
        protected TsiXmlEntryType Type { get; private set; }

        public AXmlEntry(string name, TsiXmlEntryType type)
        {
            Name = name;
            Type = type;
        }

        internal AXmlEntry(XElement rawEntry)
        {
            Name = rawEntry.Attribute("Name").Value;
            Type = getType(rawEntry);
        }


        internal abstract void Load(XElement rawEntry);
        internal abstract void Save(XElement rawParent);


        public static AXmlEntry Parse(XElement rawEntry)
        {
            var type = getType(rawEntry);
            switch (type)
            {
                case TsiXmlEntryType.Boolean:
                    return new BoolXmlEntry(rawEntry);
                case TsiXmlEntryType.Integer:
                    return new IntXmlEntry(rawEntry);
                case TsiXmlEntryType.Float:
                    return new FloatXmlEntry(rawEntry);
                case TsiXmlEntryType.String:
                    return new StringXmlEntry(rawEntry);
                case TsiXmlEntryType.ListOfBoolean:
                    return new ListOfBoolXmlEntry(rawEntry);
                case TsiXmlEntryType.ListOfInteger:
                    return new ListOfIntegerXmlEntry(rawEntry);
                case TsiXmlEntryType.ListOfFloat:
                    return new ListOfFloatXmlEntry(rawEntry);
                case TsiXmlEntryType.ListOfString:
                    return new ListOfStringXmlEntry(rawEntry);
                default:
                    break;
            }
            return null;
        }

        private static TsiXmlEntryType getType(XElement rawEntry)
        {
            return (TsiXmlEntryType)Enum.Parse(typeof(TsiXmlEntryType), rawEntry.Attribute("Type").Value);
        }
    }
}
