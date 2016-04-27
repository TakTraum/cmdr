using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdr.Editor.AppSettings
{
    public class CmdrSettings
    {
        private readonly Configuration _config;
        private readonly KeyValueConfigurationCollection _settings;


        private static CmdrSettings _instance;
        public static CmdrSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CmdrSettings();
                return _instance;
            }
        }

        public bool Initialized { get { return !String.IsNullOrEmpty(TraktorVersion); } }

        public string DefaultWorkspace
        {
            get { return getSetting("DefaultWorkspace"); }
            set { setSetting("DefaultWorkspace", value); }
        }

        public string PathToControllerDefaultMappings
        {
            get { return getSetting("PathToControllerDefaultMappings"); }
            set { setSetting("PathToControllerDefaultMappings", value); }
        }

        public string PathToTraktorSettings
        {
            get { return getSetting("PathToTraktorSettings"); }
            set { setSetting("PathToTraktorSettings", value); }
        }

        public string TraktorVersion
        {
            get { return getSetting("TraktorVersion"); }
            set { setSetting("TraktorVersion", value); }
        }


        private CmdrSettings()
        {
            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            _settings = _config.AppSettings.Settings;
        }


        public void Save()
        {
            _config.Save();
        }


        private string getSetting(string key)
        {
            if (_settings[key] == null)
                return null;
            return _settings[key].Value;
        }

        private void setSetting(string key, string value)
        {
            if (_settings[key] == null)
                _settings.Add(key, value);
            else
                _settings[key].Value = value;
        }

        private T getSection<T>(string sectionName) where T: ConfigurationSection, new()
        {
            if (_config.GetSection(sectionName) == null)
                _config.Sections.Add(sectionName, new T());
            return _config.GetSection(sectionName) as T;
        }
    }
}
