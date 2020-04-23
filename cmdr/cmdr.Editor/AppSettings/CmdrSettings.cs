using cmdr.Editor.Utils.Configuration;
using System;
using System.Configuration;

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

        public bool Initialized {
            get {
                bool ret = !String.IsNullOrEmpty(TraktorVersion);
                return ret;
            }
        }

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

        public bool OptimizeFXList
        {
            get {
                string ret = getSetting("OptimizeFXList");
                return (ret == "True");

                }
            set {
                string ret = value.ToString();
                setSetting("OptimizeFXList", ret);
            }
        }

        public bool RemoveUnusedMIDIDefinitions
        {
            get
            {
                string ret = getSetting("RemoveUnusedMIDIDefinitions");
                return (ret == "True");

            }
            set
            {
                string ret = value.ToString();
                setSetting("RemoveUnusedMIDIDefinitions", ret);
            }
        }

        public bool LoadLastFileAtStartup
        {
            get
            {
                string ret = getSetting("LoadLastFileAtStartup");
                return (ret == "True");

            }
            set
            {
                string ret = value.ToString();
                setSetting("LoadLastFileAtStartup", ret);
            }
        }


        public bool ShowDecimalNotes
        {
            get
            {
                string ret = getSetting("ShowDecimalNotes");
                return (ret == "True");

            }
            set
            {
                string ret = value.ToString();
                setSetting("ShowDecimalNotes", ret);
            }
        }

        public bool ClearFilterAtModifications
        {
            get
            {
                string ret = getSetting("ClearFilterAtModifications");
                return (ret == "True");

            }
            set
            {
                string ret = value.ToString();
                setSetting("ClearFilterAtModifications", ret);
            }
        }

        public bool ClearFilterAtPageChanges
        {
            get
            {
                string ret = getSetting("ClearFilterAtPageChanges");
                return (ret == "True");

            }
            set
            {
                string ret = value.ToString();
                setSetting("ClearFilterAtPageChanges", ret);
            }
        }

        public bool ShowNotesBeforeCC
        {
            get
            {
                string ret = getSetting("ShowNotesBeforeCC");
                return (ret == "True");

            }
            set
            {
                string ret = value.ToString();
                setSetting("ShowNotesBeforeCC", ret);
            }
        }

         public bool ConfirmDeleteDevices
        {
            get
            {
                string ret = getSetting("ConfirmDeleteDevices");
                return (ret == "True");

            }
            set
            {
                string ret = value.ToString();
                setSetting("ConfirmDeleteDevices", ret);
            }
        }

        public int FilterMenuSize
        {
            get
            {
                string st = getSetting("FilterMenuSize");

                if (Int32.TryParse(st, out int ret))
                {
                    return ret;
                }
                else
                {
                    return 20;
                }

            }
            set
            {
                string ret = value.ToString();
                setSetting("FilterMenuSize", ret);

            }
        }


        public MruSection MRU
        {
            get { return getSection<MruSection>(); }
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

        private T getSection<T>() where T : ConfigurationSection, new()
        {
            ConfigurationSectionAttribute csa = Attribute.GetCustomAttribute(typeof(T), typeof(ConfigurationSectionAttribute)) as ConfigurationSectionAttribute;
            string sectionName = (csa != null) ? csa.Name : typeof(T).Name;
            if (_config.GetSection(sectionName) == null)
                _config.Sections.Add(sectionName, new T());
            return _config.GetSection(sectionName) as T;
        }
    }
}
