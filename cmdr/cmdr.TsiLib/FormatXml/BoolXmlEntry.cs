using cmdr.TsiLib.FormatXml.Base;
using System.Xml.Linq;

namespace cmdr.TsiLib.FormatXml
{
    public class BoolXmlEntry : AValueXmlEntry<bool>
    {
        internal BoolXmlEntry(string name)
            : base(name, TsiXmlEntryType.Boolean)
        {

        }

        internal BoolXmlEntry(XElement rawEntry)
            : base(rawEntry)
        {

        }


        protected override bool Decode(string value)
        {
            return value == "0" ? false : true;
        }

        protected override string Encode(bool value)
        {
            return value ? "1" : "0";
        }
    }
}
