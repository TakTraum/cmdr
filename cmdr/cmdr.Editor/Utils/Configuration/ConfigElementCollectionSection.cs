using System.Configuration;

namespace cmdr.Editor.Utils.Configuration
{
    public class ConfigElementCollectionSection<T> : ConfigurationSection where T : ConfigurationElement, new()
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public ConfigElementCollection<T> Elements
        {
            get { return (ConfigElementCollection<T>)this[""]; }
        }
    }
}
