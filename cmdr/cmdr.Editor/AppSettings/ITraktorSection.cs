using cmdr.Editor.AppSettings.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdr.Editor.AppSettings
{
    public interface ITraktorSection
    {
        ConfigElementCollection<InstalledTraktorVersionConfigElement> InstalledVersions { get; }
        string SelectedVersion { get; set; }
        string TraktorFolder { get; }
    }
}
