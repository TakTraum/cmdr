using cmdr.TsiLib.FormatXml.Base;
using System.Xml.Linq;

namespace cmdr.TsiLib.FormatXml
{
    public class ListOfBoolXmlEntry : AListXmlEntry<bool>
    {
        internal ListOfBoolXmlEntry(string name)
            : base(name, TsiXmlEntryType.ListOfBoolean)
        {

        }

        internal ListOfBoolXmlEntry(XElement rawEntry)
            : base(rawEntry)
        {

        }


        protected override bool DecodeListItem(string value)
        {
            return value == "0" ? false : true;
        }

        protected override string EncodeListItem(bool value)
        {
            return value ? "1" : "0";
        }
    }
}
