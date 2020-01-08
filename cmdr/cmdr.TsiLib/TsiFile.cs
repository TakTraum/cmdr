using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.EventArgs;
using cmdr.TsiLib.Format;
using cmdr.TsiLib.FormatXml;
using cmdr.TsiLib.FormatXml.Interpretation;
using cmdr.TsiLib.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace cmdr.TsiLib
{
    public class TsiFile
    {
        private static readonly Regex REGEX_TRAKTOR_FOLDER = new Regex(@"Traktor ([0-9\.]+)");

        public static event EventHandler<EffectIdentificationRequest> EffectIdentificationRequest;


        public bool IsTraktorSettings { get { return Path != null && new FileInfo(Path).Name == TraktorSettings.TRAKTOR_SETTINGS_FILENAME; } }

        public string TraktorVersion { get; private set; }

        private DeviceMappingsContainer _devicesContainer;

        public string Path { get; private set; }

        private List<Device> _devices = new List<Device>();
        public IReadOnlyCollection<Device> Devices { get { return _devices.AsReadOnly(); } }

        public FxSettings FxSettings { get; private set; }

        private bool _ignoreFx;


        private TsiFile(string traktorVersion)
        {
            TraktorVersion = traktorVersion;

            _devices = new List<Device>();
            _devicesContainer = new DeviceMappingsContainer();
        }


        /// <summary>
        /// Creates a new TSI File for the specified version of Traktor.
        /// </summary>
        /// <param name="traktorVersion">The targeted Traktor version.</param>
        public static TsiFile Create(string traktorVersion)
        {
            return new TsiFile(traktorVersion);
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
                TsiXmlDocument xml = new TsiXmlDocument(filePath);
                file.load(xml);
                return file;
            }
            catch (Exception e)
            {
                String ret = e.ToString();
                //System.Windows.Forms.MessageBox.ShowError("Cannot open file."+ret);   // How to pass the exception string to the message box?
                return null;
            }
        }


        public Device CreateDevice(string deviceTypeStr)
        {
            return new Device(createNewId(), deviceTypeStr, TraktorVersion);
        }

        public void AddDevice(Device device)
        {
            InsertDevice(Devices.Count, device);
        }

        public void InsertDevice(int index, Device device)
        {
            insertDevice(index, device, false);
        }

        public void MoveDevice(int oldIndex, int newIndex)
        {
            var temp = _devices[oldIndex];
            RemoveDevice(temp.Id);
            insertDevice(newIndex, temp, true);
        }

        public void RemoveDevice(int deviceId)
        {
            var device = _devices.Single(d => d.Id == deviceId);
            _devices.Remove(device);
            _devicesContainer.Devices.List.Remove(device.RawDevice);
        }

        public bool Save(string filePath)
        {
            // workaround to save indices instead of ids
            var effectSelectorInCommands = getCriticalEffectSelectorInCommands();
            var effectSelectorOutCommands = getCriticalEffectSelectorOutCommands();

            bool prepared = false;

            if (!_ignoreFx && (effectSelectorInCommands.Any() || effectSelectorOutCommands.Any()))
            {
                if (FxSettings == null)
                    FxSettings = (TraktorSettings.Initialized) ? TraktorSettings.Instance.FxSettings : createDefaultFxSettings();

                prepareFxForSave(effectSelectorInCommands, effectSelectorOutCommands);
                prepared = true;
            }

            // build controller config (binary)
            DeviceIoConfigController controllerConfig = null;
            try
            {
                string tsiData = getDataAsBase64String();
                controllerConfig = new DeviceIoConfigController();
                controllerConfig.Value = tsiData;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error building controller config. Reason: " + ex.Message);
                return false;
            }

            if (prepared)
                restoreEffectSelectorCommands(effectSelectorInCommands, effectSelectorOutCommands);

            // build xml document
            try
            {
                TsiXmlDocument xml = (Path != null) ? new TsiXmlDocument(Path) : new TsiXmlDocument();
                if (FxSettings != null && (effectSelectorInCommands.Any() || effectSelectorOutCommands.Any()))
                    FxSettings.Save(xml);
                xml.SaveEntry(controllerConfig);
                xml.Save(filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error building xml document. Reason: " + ex.Message);
                return false;
            }

            Path = filePath;
            return true;
        }


        private void load(TsiXmlDocument xml)
        {
            // Traktor version, optional (only for "Traktor Settings.tsi")
            var browserDirRoot = xml.GetEntry<BrowserDirRoot>();
            if (browserDirRoot != null)
            {
                Match m = REGEX_TRAKTOR_FOLDER.Match(browserDirRoot.Value);
                if (m.Success) // Overwrite version if possible
                    TraktorVersion = m.Groups[1].Value;
            }

            // effects, optional (FxSettings.Load may return null)
            FxSettings = FxSettings.Load(xml);

            // devices
            var controllerConfig = xml.GetEntry<DeviceIoConfigController>();
            if (controllerConfig != null)
            {
                byte[] decoded = Convert.FromBase64String(controllerConfig.Value);
                _devicesContainer = new DeviceMappingsContainer(new MemoryStream(decoded));
                int id = 0;
                _devices = _devicesContainer.Devices.List.Select(d => new Device(id++, d)).ToList();

                var effectSelectorInCommands = getCriticalEffectSelectorInCommands();
                var effectSelectorOutCommands = getCriticalEffectSelectorOutCommands();
                if (effectSelectorInCommands.Any() || effectSelectorOutCommands.Any())
                {
                    // need FxSettings for interpretation but not provided by file itself?
                    if (FxSettings == null)
                    {
                        // call for help
                        string rId = new FileInfo(Path).Name;
                        var request = new EffectIdentificationRequest(rId);
                        var handler = EffectIdentificationRequest;
                        if (handler != null)
                        {
                            handler(this, request);

                            // wait for help
                            while (!request.Handled)
                                Thread.Sleep(100);

                            if (request.FxSettings != null)
                                FxSettings = request.FxSettings;
                        }
                    }

                    // if possible, replace effect indices with ids
                    if (FxSettings != null)
                        restoreEffectSelectorCommands(effectSelectorInCommands, effectSelectorOutCommands);
                    else
                        _ignoreFx = true;
                }
            }
        }

        private FxSettings createDefaultFxSettings()
        {
            return new FxSettings(new List<Effect>(), new Dictionary<Effect, FxSnapshot>());
        }

        private List<EffectSelectorInCommand> getCriticalEffectSelectorInCommands()
        {
            var effectCommands = Devices
                .SelectMany(d => d.Mappings.Select(m => m.Command))
                .Where(c => (c is EffectSelectorInCommand) && (c.InteractionMode == MappingInteractionMode.Direct || c.InteractionMode == MappingInteractionMode.Hold));
            return effectCommands.Cast<EffectSelectorInCommand>().ToList();
        }

        private List<EffectSelectorOutCommand> getCriticalEffectSelectorOutCommands()
        {
            var effectCommands = Devices
                .SelectMany(d => d.Mappings.Select(m => m.Command))
                .Where(c => c is EffectSelectorOutCommand)
                .Cast<EffectSelectorOutCommand>()
                .Where(e => !e.AllEffects);
            return effectCommands.ToList();
        }

        private void prepareFxForSave(IEnumerable<EffectSelectorInCommand> effectSelectorInCommands, IEnumerable<EffectSelectorOutCommand> effectSelectorOutCommands)
        {
            prepareFxSettings(effectSelectorInCommands, effectSelectorOutCommands);

            // replace ids with indices
            foreach (var e in effectSelectorInCommands)
                if (e.Value != Effect.NoEffect)
                    e.Value = (Effect)(FxSettings.Effects.IndexOf(e.Value) + 1);

            foreach (var e in effectSelectorOutCommands)
            {
                if (e.ControllerRangeMin != Effect.NoEffect)
                    e.ControllerRangeMin = (Effect)(FxSettings.Effects.IndexOf(e.ControllerRangeMin) + 1);
                if (e.ControllerRangeMax != Effect.NoEffect)
                    e.ControllerRangeMax = (Effect)(FxSettings.Effects.IndexOf(e.ControllerRangeMax) + 1);
            }
        }

        private void prepareFxSettings(IEnumerable<EffectSelectorInCommand> effectSelectorInCommands, IEnumerable<EffectSelectorOutCommand> effectSelectorOutCommands)
        {
            List<Effect> usedFxIn = effectSelectorInCommands.Select(e => e.Value).Distinct().ToList();
            List<Effect> usedFxOut = effectSelectorOutCommands.Select(e => e.ControllerRangeMin).Distinct().ToList();
            List<Effect> usedFx = usedFxIn.Union(usedFxOut).Distinct().Except(new[] { Effect.NoEffect }).OrderBy(e => e).ToList();

            // Keep effects from Traktor settings as they are. Append new effects if necessary.
            if (IsTraktorSettings)
                usedFx = FxSettings.Effects.Union(usedFx).Distinct().ToList();

            Dictionary<Effect, FxSnapshot> usedSnapshots = new Dictionary<Effect, FxSnapshot>();
            foreach (var fx in usedFx)
            {
                FxSnapshot snapshot = null;
                if (FxSettings.Snapshots.ContainsKey(fx))
                    snapshot = FxSettings.Snapshots[fx];
                else
                {
                    if (TraktorSettings.Initialized && TraktorSettings.Instance.FxSettings.Snapshots.ContainsKey(fx))
                        snapshot = TraktorSettings.Instance.FxSettings.Snapshots[fx];
                    else
                        snapshot = new FxSnapshot(fx);
                }
                usedSnapshots.Add(fx, snapshot);
            }

            // Keep snapshots from Traktor settings as they are. Add new snapshots if necessary.
            if (IsTraktorSettings)
                usedSnapshots = FxSettings.Snapshots.Union(usedSnapshots).Distinct().ToDictionary(s => s.Key, s => s.Value);

            FxSettings = new FxSettings(usedFx, usedSnapshots);
        }

        private void restoreEffectSelectorCommands(IEnumerable<EffectSelectorInCommand> effectSelectorInCommands, IEnumerable<EffectSelectorOutCommand> effectSelectorOutCommands)
        {
            // replace indices with ids
            foreach (var e in effectSelectorInCommands)
                if (e.Value != Effect.NoEffect)
                    e.Value = FxSettings.Effects[(int)e.Value - 1];

            foreach (var e in effectSelectorOutCommands)
            {
                if (e.ControllerRangeMin != Effect.NoEffect)
                    e.ControllerRangeMin = FxSettings.Effects[(int)e.ControllerRangeMin - 1];
                if (e.ControllerRangeMax != Effect.NoEffect)
                    e.ControllerRangeMax = FxSettings.Effects[(int)e.ControllerRangeMax - 1];
            }
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

        private void insertDevice(int index, Device device, bool asIs)
        {
            if (device.Id < 0 || !asIs)
                device.Id = createNewId();

            if (index == Devices.Count)
            {
                _devices.Add(device);
                _devicesContainer.Devices.List.Add(device.RawDevice);
            }
            else
            {
                _devices.Insert(index, device);
                _devicesContainer.Devices.List.Insert(index, device.RawDevice);
            }
        }
    }
}
