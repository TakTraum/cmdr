using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace cmdr.TsiLib.FormatXml.Base
{
    public abstract class AListXmlEntry<T> : AValueXmlEntry<List<T>>
    {
        internal AListXmlEntry(string name, TsiXmlEntryType type)
            : base(name, type)
        {
            Value = new List<T>();
        }

        internal AListXmlEntry(XElement rawEntry)
            : base(rawEntry)
        {

        }


        protected sealed override List<T> Decode(string value)
        {
            return value.Split(';').Select(s => DecodeListItem(s)).ToList();
        }

        protected sealed override string Encode(List<T> value)
        {
            return String.Join(";", value.Select(t => EncodeListItem(t)));
        }


        protected abstract T DecodeListItem(string value);
        protected abstract string EncodeListItem(T value);
    }
}
