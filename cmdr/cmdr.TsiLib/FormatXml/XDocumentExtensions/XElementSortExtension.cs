using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace cmdr.TsiLib.FormatXml.XDocumentExtensions
{
    public static class XElementSortExtension
    {
        public static void Sort(this XElement source)
        {
            //Make sure there is a valid source
            if (source == null) throw new ArgumentNullException("source");

            //Sort the children IF any exist
            List<XElement> sortedChildren = source.Elements().OrderBy(e => e.Attribute("Name").Value).ToList();
            if (source.HasElements)
            {
                source.RemoveNodes();
                sortedChildren.ForEach(c => c.Sort());
                sortedChildren.ForEach(c => source.Add(c));
            }
        }
    }
}
