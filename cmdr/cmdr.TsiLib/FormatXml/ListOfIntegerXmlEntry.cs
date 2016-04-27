using cmdr.TsiLib.FormatXml.Base;
using System.Xml.Linq;

namespace cmdr.TsiLib.FormatXml
{
    public class ListOfIntegerXmlEntry : AListXmlEntry<int>
    {
        internal ListOfIntegerXmlEntry(string name)
            : base(name, TsiXmlEntryType.ListOfInteger)
        {

        }

        internal ListOfIntegerXmlEntry(XElement rawEntry)
            : base(rawEntry)
        {

        }


        protected override int DecodeListItem(string value)
        {
            return int.Parse(value);
        }

        protected override string EncodeListItem(int value)
        {
            return value.ToString();
        }
    }
}
