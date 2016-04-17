using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using cmdr.TsiLib.Format;
using cmdr.TsiLib.Utils;

namespace cmdr.TsiLib
{
    public class TsiFile
    {
        private static readonly string XPATH_TO_DATA = "/NIXML/TraktorSettings/Entry[starts-with(@Name, 'DeviceIO.Config.')]";

        private string _traktorVersion;
        private DeviceMappingsContainer _devicesContainer;

        public string Path { get; private set; }

        private List<Device> _devices = new List<Device>();
        public IReadOnlyCollection<Device> Devices { get { return _devices.AsReadOnly(); } }


        /// <summary>
        /// Creates a new TSI File for specified Traktor Version.
        /// </summary>
        /// <param name="traktorVersion">The targeted Traktor Version.</param>
        public TsiFile(string traktorVersion)
        {
            _traktorVersion = traktorVersion;

            _devices = new List<Device>();
            _devicesContainer = new DeviceMappingsContainer();
        }

        /// <summary>
        /// Loads a TSI File.
        /// </summary>
        /// <exception cref="System.Exception">Thrown when file cannot be loaded or parsed.</exception>
        /// <param name="traktorVersion">The targeted Traktor Version. The version of the file is checked against it.</param>
        /// <param name="filePath">Path of the file.</param>
        public static TsiFile Load(string traktorVersion, string filePath)
        {
            TsiFile file = new TsiFile(traktorVersion);
            file.Path = filePath;
            try
            {
                using (Stream source = getFileReadStream(filePath))
                    file.load(XDocument.Load(source));
                return file;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public Device CreateDevice(string deviceTypeStr)
        {
            return new Device(createNewId(), deviceTypeStr, _traktorVersion);
        }

        public void AddDevice(Device device)
        {
            _devices.Add(device);
            _devicesContainer.Devices.List.Add(device.RawDevice);
        }

        public void RemoveDevice(int deviceId)
        {
            var device = _devices.Single(d => d.Id == deviceId);
            _devices.Remove(device);
            _devicesContainer.Devices.List.Remove(device.RawDevice);
        }

        public bool Save(string filePath)
        {
            try
            {
                string tsiData = getDataAsBase64String();

                string tsi;
                using (StreamReader source = new StreamReader((Path != null) ? getFileReadStream(Path) : getTemplateStream()))
                    tsi = source.ReadToEnd();

                tsi = Regex.Replace(tsi,
                  "<Entry Name=\"DeviceIO.Config.(.*)\"(.*)Value=\".*\"",
                  String.Format("<Entry Name=\"DeviceIO.Config.$1\"$2Value=\"{0}\"", tsiData));

                using (StreamWriter destination = new StreamWriter(getFileWriteStream(filePath)))
                    destination.Write(tsi);
            }
            catch (Exception)
            {
                return false;
            }

            Path = filePath;
            return true;
        }


        private void load(XDocument doc)
        {
            XElement element = doc.XPathSelectElement(XPATH_TO_DATA);
            
            // Does not matter. Default is 3. 
            //DeviceType type = (DeviceType)Convert.ToInt32(element.Attribute("Type").Value); 
            
            string data = element.Attribute("Value").Value;

            byte[] decoded = Convert.FromBase64String(data);

            _devicesContainer = new DeviceMappingsContainer(new MemoryStream(decoded));
            
            int id = 0;
            _devices = _devicesContainer.Devices.List.Select(d => new Device(id++, d)).ToList();
        }

        private string getDataAsBase64String()
        {
            byte[] data = null;
            using (MemoryStream stream = new MemoryStream())
            {
                // write DevicesContainer into memory stream
                _devicesContainer.Write(new Writer(stream));

                // get all bytes from memory stream
                data = stream.ToBytes();
            }

            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return Convert.ToBase64String(data, Base64FormattingOptions.None);
        }

        private Stream getTemplateStream()
        {
            return EmbeddedResource.Get("Template.tsi");
        }

        private static Stream getFileReadStream(string filePath)
        {
            return File.Open(filePath, FileMode.Open, FileAccess.Read);
        }

        private static Stream getFileWriteStream(string filePath)
        {
            return File.Open(filePath, FileMode.Create, FileAccess.Write);
        }

        private int createNewId()
        {
            var devices = _devices;
            int currMaxId = devices.Any() ? devices.Max(b => b.Id) : 0;
            if (currMaxId < int.MaxValue)
                return currMaxId + 1;
            else
            {
                // search for first unused id
                for (int i = 0; i < int.MaxValue; i++)
                {
                    if (_devices.Any(m => m.Id == i))
                        continue;
                    return i;
                }
            }
            throw new Exception("Devices are full!");
        }
    }
}
