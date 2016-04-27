using cmdr.TsiLib.FormatXml.Base;
using System.Globalization;
using System.Xml.Linq;

namespace cmdr.TsiLib.FormatXml
{
    public class ListOfFloatXmlEntry : AListXmlEntry<float>
    {
        internal ListOfFloatXmlEntry(string name)
            : base(name, TsiXmlEntryType.ListOfFloat)
        {

        }

        internal ListOfFloatXmlEntry(XElement rawEntry)
            : base(rawEntry)
        {

        }        


        protected override float DecodeListItem(string value)
        {
            return float.Parse(value, CultureInfo.InvariantCulture);
        }

        protected override string EncodeListItem(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
