using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdr.Editor.AppSettings.Base
{
    public abstract class ConfigElement : ConfigurationElement
    {
        public abstract string Key { get; }
    }
}
