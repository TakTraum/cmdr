using cmdr.TsiLib.FormatXml.Base;
using System;
using System.Xml.Linq;

namespace cmdr.TsiLib.FormatXml
{
    public class EnumXmlEntry<T> : AValueXmlEntry<T> where T: struct, IConvertible
    {
        internal EnumXmlEntry(string name)
            : base(name, TsiXmlEntryType.ListOfInteger)
        {

        }

        internal EnumXmlEntry(XElement rawEntry)
            : base(rawEntry)
        {

        }


        protected override T Decode(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        protected override string Encode(T value)
        {
            return Convert.ToInt32(value).ToString();
        }
    }
}
