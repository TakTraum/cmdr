using cmdr.TsiLib.FormatXml.Base;
using System.Xml.Linq;

namespace cmdr.TsiLib.FormatXml
{
    public class ListOfStringXmlEntry : AListXmlEntry<string>
    {
        internal ListOfStringXmlEntry(string name)
            : base(name, TsiXmlEntryType.ListOfString)
        {

        }

        internal ListOfStringXmlEntry(XElement rawEntry)
            : base(rawEntry)
        {

        }


        protected override string DecodeListItem(string value)
        {
            return value;
        }

        protected override string EncodeListItem(string value)
        {
            return value;
        }
    }
}
