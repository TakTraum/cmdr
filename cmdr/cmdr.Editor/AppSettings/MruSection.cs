using cmdr.Editor.Utils.Configuration;
using System.Configuration;

namespace cmdr.Editor.AppSettings
{
    [ConfigurationSection("MRU")]
    public class MruSection : ConfigElementCollectionSection<MruElement>
    {

    }

    [ConfigurationElement("file")]
    public class MruElement : ConfigurationElement
    {
        [ConfigurationProperty("path", IsRequired = true, IsKey = true)]
        public string Path
        {
            get { return (string)this["path"]; }
            set { this["path"] = value; }
        }
    }
}
