using cmdr.TsiLib.FormatXml.Base;
using System.Xml.Linq;

namespace cmdr.TsiLib.FormatXml
{
    public class IntXmlEntry : AValueXmlEntry<int>
    {
        internal IntXmlEntry(string name)
            : base(name, TsiXmlEntryType.Integer)
        {

        }

        internal IntXmlEntry(XElement rawEntry)
            : base(rawEntry)
        {

        }


        protected override int Decode(string value)
        {
            return int.Parse(value);
        }

        protected override string Encode(int value)
        {
            return value.ToString();
        }
    }
}
