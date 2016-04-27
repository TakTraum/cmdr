using cmdr.TsiLib.FormatXml.Base;
using System.Globalization;
using System.Xml.Linq;

namespace cmdr.TsiLib.FormatXml
{
    public class FloatXmlEntry : AValueXmlEntry<float>
    {
        internal FloatXmlEntry(string name)
            : base(name, TsiXmlEntryType.Float)
        {

        }

        internal FloatXmlEntry(XElement rawEntry)
            : base(rawEntry)
        {

        }


        protected override float Decode(string value)
        {
            return float.Parse(value, CultureInfo.InvariantCulture);
        }

        protected override string Encode(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
