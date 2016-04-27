using cmdr.TsiLib.FormatXml.Base;
using System;
using System.Xml.Linq;

namespace cmdr.TsiLib.FormatXml
{
    public class ListOfEnumXmlEntry<T> : AListXmlEntry<T> where T: struct, IConvertible
    {
        internal ListOfEnumXmlEntry(string name)
            : base(name, TsiXmlEntryType.ListOfInteger)
        {

        }

        internal ListOfEnumXmlEntry(XElement rawEntry)
            : base(rawEntry)
        {

        }
        
        
        protected override T DecodeListItem(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        protected override string EncodeListItem(T value)
        {
            return Convert.ToInt32(value).ToString();
        }
    }
}
