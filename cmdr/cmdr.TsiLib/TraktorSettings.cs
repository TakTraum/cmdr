using cmdr.TsiLib.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace cmdr.TsiLib
{
    public class TraktorSettings
    {
        private static readonly string XPATH_TO_EFFECTS = "/NIXML/TraktorSettings/Entry[@Name='Audio.FX.Selection']";

        private static TraktorSettings _instance;
        public static TraktorSettings Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("TraktorSettings not initialized! Call Initialize(string filePath) first.");
                return _instance;
            }
        }

        public string Path { get; private set; }

        private List<Effect> _effects = new List<Effect>();
        public IReadOnlyCollection<Effect> Effects
        {
            get { return _effects.AsReadOnly(); }
        }


        private TraktorSettings(XDocument doc)
        {
            XElement element = doc.XPathSelectElement(XPATH_TO_EFFECTS);
            string valueStr = element.Attribute("Value").Value;

            string[] effectStrings = valueStr.Split(';');
            _effects = effectStrings.Select(e => (Effect)int.Parse(e)).ToList();
        }


        public static bool Initialize(string filePath)
        {
            try
            {
                using (Stream source = getFileReadStream(filePath))
                {
                    XDocument doc = XDocument.Load(source);
                    TraktorSettings result = new TraktorSettings(doc);
                    result.Path = filePath;
                    _instance = result;
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        private static Stream getFileReadStream(string filePath)
        {
            return File.Open(filePath, FileMode.Open, FileAccess.Read);
        }

    }
}
