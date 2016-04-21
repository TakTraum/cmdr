using cmdr.Editor.AppSettings.Base;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdr.Editor.AppSettings
{
    public class TraktorSection : ConfigSection, ITraktorSection
    {
        [ConfigurationProperty("installedVersions")]
        public ConfigElementCollection<InstalledTraktorVersionConfigElement> InstalledVersions
        {
            get
            {
                return this["installedVersions"] as ConfigElementCollection<InstalledTraktorVersionConfigElement>;
            }
        }

        [ConfigurationProperty("selectedVersion")]
        public string SelectedVersion
        {
            get { return this["selectedVersion"].ToString(); }
            set { this["selectedVersion"] = value; }
        }

        public string TraktorFolder
        {
            get { return InstalledVersions.Where(v => v.Version.Equals(SelectedVersion)).Select(v => v.Path).FirstOrDefault(); }
        }


    }
}
