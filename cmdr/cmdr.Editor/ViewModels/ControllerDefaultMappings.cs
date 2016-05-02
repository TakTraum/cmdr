using cmdr.Editor.AppSettings;
using cmdr.TsiLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Runtime.Serialization;
using cmdr.Editor.Utils;
using System.Diagnostics;

namespace cmdr.Editor.ViewModels
{
    [KnownType(typeof(ControllerDefaultMappingFile))]
    [CollectionDataContract(Name = "DefaultMappings", Namespace="")]
    public class ControllerDefaultMappings : List<ControllerDefaultMappings.ControllerDefaultMappingFile>
    {

        [DataContract(Name = "Mapping", Namespace="")]
        public class ControllerDefaultMapping
        {
            [DataMember]
            public string DeviceTypeStr { get; private set; }

            public ControllerDefaultMappingFile File { get; set; }
            public Device DefaultDevice { get; private set; }


            public ControllerDefaultMapping(ControllerDefaultMappingFile file, string deviceTypeStr)
            {
                File = file;
                DeviceTypeStr = deviceTypeStr;
                if (File.TsiFile != null)
                    DefaultDevice = File.TsiFile.Devices.FirstOrDefault(d => d.TypeStr.Equals(DeviceTypeStr));
            }


            public async Task<Device> LoadAsync()
            {
                if (File == null)
                    return null;
                var tsi = await File.LoadAsync();
                if (tsi != null)
                    DefaultDevice = tsi.Devices.FirstOrDefault(d => d.TypeStr.Equals(DeviceTypeStr));
                else
                    DefaultDevice = null;
                return DefaultDevice;
            }
        }


        [KnownType(typeof(ControllerDefaultMapping))]
        [DataContract(Name = "File", Namespace = "")]
        public class ControllerDefaultMappingFile
        {
            [DataMember]
            public string Manufacturer { get; private set; }

            [DataMember]
            public string Controller { get; private set; }

            [DataMember]
            public string Path { get; private set; }

            private List<ControllerDefaultMapping> _defaultMappings = new List<ControllerDefaultMapping>();
            [DataMember]
            public List<ControllerDefaultMapping> DefaultMappings { get { return _defaultMappings; } set { _defaultMappings = value; } }

            public TsiFile TsiFile { get; private set; }


            public ControllerDefaultMappingFile(string manufacturer, string controller, string path)
            {
                Manufacturer = manufacturer;
                Controller = controller;
                Path = path;
            }

            public async Task<TsiFile> LoadAsync()
            {
                TsiFile = await Task<TsiFile>.Factory.StartNew(() => TsiFile.Load(CmdrSettings.Instance.TraktorVersion, Path));
                return TsiFile;
            }
        }



        public static readonly string PATH_TO_CACHE_FOLDER = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");
        public static readonly string PATH_TO_CACHE_FILE = Path.Combine(PATH_TO_CACHE_FOLDER, "defaultMappings.xml");

        private static Dictionary<string, ControllerDefaultMapping> _dict = new Dictionary<string, ControllerDefaultMapping>();
        private static int _progressCounter;


        private ControllerDefaultMappings() { }

        private static ControllerDefaultMappings _instance;
        public static ControllerDefaultMappings Instance
        {
            get { return _instance ?? (_instance = load() ?? new ControllerDefaultMappings()); }
        }


        public async Task LoadAsync(string rootPath)
        {            
            if (String.IsNullOrEmpty(rootPath))
                return;

            _dict.Clear();
            _progressCounter = 0;

            App.SetStatus("Loading defaults for proprietary devices ... 0%");

            try
            {
                DirectoryInfo di = new DirectoryInfo(rootPath);
                var allFiles = di.EnumerateDirectories().SelectMany(d => d.EnumerateFiles());

                // remove missing files from cache
                for (int i = Count - 1; i >= 0; i--)
                {
                    if (!allFiles.Any(f => f.FullName.Equals(this[i].Path)))
                        RemoveAt(i);
                }

                // process and cache new files
                var files = allFiles.Where(f => !this.Any(ignored => ignored.Path == f.FullName)).ToList();
                var tasks = files.Select(f => addDefaultMappings(files.Count, f.FullName));
                await Task.WhenAll(tasks);

                save();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            _dict = this.SelectMany(cdm => cdm.DefaultMappings.Where(dm => dm.DeviceTypeStr != Device.TYPE_STRING_GENERIC_MIDI))
                .DistinctBy(dm => dm.DeviceTypeStr)
                .ToDictionary(dm => dm.DeviceTypeStr, dm => dm);

            App.ResetStatus();
        }

        public ControllerDefaultMapping this[string deviceTypeStr]
        {
            get
            {
                if (_dict.ContainsKey(deviceTypeStr))
                    return _dict[deviceTypeStr];
                return null;
            }
        }


        private void save()
        {
            Directory.CreateDirectory(PATH_TO_CACHE_FOLDER);
            XDocument doc = SerializationHelper.Serialize<ControllerDefaultMappings>(this, true);
            doc.Save(PATH_TO_CACHE_FILE);
        }

        private async Task addDefaultMappings(int count, string pathToDefaultFile)
        {
            FileInfo fi = new FileInfo(pathToDefaultFile);
            string manufacturer = fi.Directory.Name;
            string controller = Path.GetFileNameWithoutExtension(fi.Name)
                .Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries)
                .Last();

            TsiFile tsi = await loadTsiAsync(pathToDefaultFile);
            if (tsi == null)
            {
                Debug.WriteLine("Could not load " + pathToDefaultFile);
                return;
            }

            var file = new ControllerDefaultMappingFile(manufacturer, controller, pathToDefaultFile);
            file.DefaultMappings.AddRange (tsi.Devices.Select(pd => new ControllerDefaultMapping(file, pd.TypeStr)));
            this.Add(file);

            Interlocked.Increment(ref _progressCounter);
            App.SetStatus("Loading defaults for proprietary devices ... " + ((double)_progressCounter / count * 100) + "%");
        }

        private static ControllerDefaultMappings load()
        {
            if (!File.Exists(PATH_TO_CACHE_FILE))
                return null;

            ControllerDefaultMappings result = null;
            try
            {
                XDocument doc = XDocument.Load(PATH_TO_CACHE_FILE);
                result = SerializationHelper.Deserialize<ControllerDefaultMappings>(doc, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            if (result != null)
            {
                // restore references
                foreach (var file in result)
                    foreach (var m in file.DefaultMappings)
                        m.File = file;
            }

            return result;
        }

        private static async Task<TsiFile> loadTsiAsync(string filePath)
        {
            return await Task<TsiFile>.Factory.StartNew(() => TsiFile.Load(CmdrSettings.Instance.TraktorVersion, filePath));
        }

        private static XElement stripNamespace(XElement root)
        {
            return new XElement(
                root.Name.LocalName,
                root.HasElements ?
                    root.Elements().Select(el => stripNamespace(el)) :
                    (object)root.Value
            );
        }
    }
}
