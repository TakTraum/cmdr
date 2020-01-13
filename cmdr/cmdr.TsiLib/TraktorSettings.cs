using cmdr.TsiLib.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace cmdr.TsiLib
{
    public static class TraktorSettings
    {
        public static readonly string TRAKTOR_FALLBACK_VERSION = "2.0.1 (R10169)";
        public static readonly string TRAKTOR_SETTINGS_FILENAME = "Traktor Settings.tsi";

        private static string _pathToTraktorSettingsTsi = null;

        public static bool Initialized { get { return _pathToTraktorSettingsTsi != null; } }

        private static TsiFile _instance;
        public static TsiFile Instance
        {
            get
            {
                if (!Initialized)
                    throw new InvalidOperationException("TraktorSettings not initialized! Call Initialize(string filePath) first.");

                if (_instance == null)
                {
                    if (!load())
                        throw new InvalidOperationException("TraktorSettings could not be loaded!");
                }
                return _instance;
            }
        }

        
        /// <summary>
        /// Initialize Traktor Settings with given path.
        /// </summary>
        /// <param name="filePath">Path to "Traktor Settings.tsi". Usually: ...\Native Instruments\Traktor {Version}\Traktor Settings.tsi</param>
        /// <returns>True on success, false otherwise.</returns>
        public static bool Initialize(string filePath, bool load)
        {
            if (String.IsNullOrEmpty(filePath))
                return false;

            try
            {
                FileInfo fi = new FileInfo(filePath);
                if (!fi.Exists || fi.Name != TRAKTOR_SETTINGS_FILENAME)
                    return false;                
            }
            catch (Exception)
            {
                return false;
            }

            _pathToTraktorSettingsTsi = filePath;

            if (load)
                return TraktorSettings.load();
            return true;
        }

        private static bool load()
        {
            _instance = TsiFile.Load(TRAKTOR_FALLBACK_VERSION, _pathToTraktorSettingsTsi, false);
            return (_instance != null);
        }
    }
}
