using cmdr.TsiLib.FormatXml.Base;
using System.Xml.Linq;

namespace cmdr.TsiLib.FormatXml
{
    public class StringXmlEntry : AValueXmlEntry<string>
    {
        internal StringXmlEntry(string name)
            : base(name, TsiXmlEntryType.String)
        {

        }

        internal StringXmlEntry(XElement rawEntry)
            : base(rawEntry)
        {

        }


        protected override string Decode(string value)
        {
            return value;
        }

        protected override string Encode(string value)
        {
            return value;
        }
    }
}
