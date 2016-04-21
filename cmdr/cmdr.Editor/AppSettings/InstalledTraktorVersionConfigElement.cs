using cmdr.Editor.AppSettings.Base;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdr.Editor.AppSettings
{
    public class InstalledTraktorVersionConfigElement : ConfigElement
    {
        [ConfigurationProperty("version", IsRequired = true)]
        public string Version
        {
            get { return this["version"] as string; }
            set { this["version"] = value; }
        }

        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get { return this["path"] as string; }
            set { this["path"] = value; }
        }

        public override string Key
        {
            get { return Version; }
        }
    }
}
