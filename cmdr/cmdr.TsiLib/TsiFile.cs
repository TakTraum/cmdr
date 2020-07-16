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
using System.Runtime.ExceptionServices;

/*
 * Terminology about FX list
 *                    
 * indices - what is on the TSI. Relative to own file or Trakor settigs.TSI.
 * id - actual effects, with string
 * effects: what is on the file
 * snapshots: the 43 effects
 */

namespace cmdr.TsiLib
{
    public class TsiFile
    {
        private static readonly Regex REGEX_TRAKTOR_FOLDER = new Regex(@"Traktor ([0-9\.]+)");

        public static event EventHandler<EffectIdentificationRequest> EffectIdentificationRequest;


        public bool IsTraktorSettings { get { return Path != null && new FileInfo(Path).Name == TraktorSettings.TRAKTOR_SETTINGS_FILENAME; } }

        public string TraktorVersion { get; private set; }

        public bool OptimizeFXList { get; private set; }

        private DeviceMappingsContainer _devicesContainerControllers;
        private DeviceMappingsContainer _devicesContainerKeyboard;

        public string Path { get; private set; }

        private List<Device> _devices = new List<Device>();
        public IReadOnlyCollection<Device> Devices { get { return _devices.AsReadOnly(); } }

        public FxSettings FxSettings { get; private set; }

        private bool _ignoreFx;


        private TsiFile(string traktorVersion)
        {
            TraktorVersion = traktorVersion;
            OptimizeFXList = false;            // this ONLY gets the actual value on the SAVE command

            _devices = new List<Device>();
            _devicesContainerControllers = new DeviceMappingsContainer();
            _devicesContainerKeyboard = new DeviceMappingsContainer();
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
        public static TsiFile Load(string traktorVersion, string filePath, bool RemoveUnusedMIDIDefinitions, bool ignoreExceptions = true)
        {
            TsiFile file = new TsiFile(traktorVersion);
            file.Path = filePath;
            try {
                TsiXmlDocument xml = new TsiXmlDocument(filePath);
                file.load(xml, RemoveUnusedMIDIDefinitions);
                return file;
            }
            catch (AggregateException ex) {
                if (ignoreExceptions) {
                    return null;
                } else {
                    // Preserves the stack trace 
                    // https://stackoverflow.com/questions/57383/how-to-rethrow-innerexception-without-losing-stack-trace-in-c
                    ExceptionDispatchInfo.Capture(ex.InnerException).Throw();

                    // This is just to please the compiler
                    throw ex;
                }
            }
        }

        public Device CreateDevice(string deviceTypeStr)
        {
            return new Device(createNewId(), deviceTypeStr, TraktorVersion, false, false);
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

            if (device.IsKeyboard) {
                _devicesContainerKeyboard.Devices.List.Remove(device.RawDevice);
            } else {
                _devicesContainerControllers.Devices.List.Remove(device.RawDevice);
            }
        }

        public bool Save(string filePath, bool optimizeFXList, bool backup = false)
        {
            // workaround to save indices (position on a list) instead of ids (actual command)
            var effectSelectorInCommands = getCriticalEffectSelectorInCommands();
            var effectSelectorOutCommands = getCriticalEffectSelectorOutCommands();

            OptimizeFXList = optimizeFXList;

            bool prepared = false;

            if (!_ignoreFx && (effectSelectorInCommands.Any() || effectSelectorOutCommands.Any()))
            {
                if (FxSettings == null)
                    FxSettings = (TraktorSettings.Initialized) ? TraktorSettings.Instance.FxSettings : createDefaultFxSettings();

                prepareFxForSave(effectSelectorInCommands, effectSelectorOutCommands);
                prepared = true;
            }

            // build controller config (binary)
            DeviceIoConfigController controllerConfigController = null;
            DeviceIoConfigKeyboard controllerConfigKeyboard = null;

            var all_devices = _devices.Select(d => d).ToList();
            var only_keyboard = _devices.Where(d => (d.IsKeyboard == true)).ToList();
            var only_controllers = _devices.Where(d => (d.IsKeyboard == false)).ToList();

            try {

                if (only_controllers.Any()) {
                    string tsiDataController = getDataAsBase64String(false);
                    controllerConfigController = new DeviceIoConfigController();
                    controllerConfigController.Value = tsiDataController;
                }

                if (only_keyboard.Any()) {
                    string tsiDataKeyboard = getDataAsBase64String(true);
                    controllerConfigKeyboard = new DeviceIoConfigKeyboard();
                    controllerConfigKeyboard.Value = tsiDataKeyboard;
                }
            }
            catch (Exception ex)
            {
                
                // FIXME: show this to the user somehow
                //   at least a show console option

                // Exception thrown: 'System.OutOfMemoryException' in mscorlib.dll
                // Error building controller config. Reason: Exception of type 'System.OutOfMemoryException' was thrown.

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

                if (only_controllers.Any()) {
                    xml.SaveEntry(controllerConfigController);
                }
                if (only_keyboard.Any()) {
                    xml.SaveEntry(controllerConfigKeyboard);
                }

                xml.Save(filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error building xml document. Reason: " + ex.Message);
                return false;
            }

            if (!backup) {
                Path = filePath;
            }
            return true;
        }

        int max_id = 0;



        private void load(TsiXmlDocument xml, bool RemoveUnusedMIDIDefinitions)
        {
            // Traktor version, optional (only for "Traktor Settings.tsi")
            var browserDirRoot = xml.GetEntry<BrowserDirRoot>();
            if (browserDirRoot != null) 
            {
                Match m = REGEX_TRAKTOR_FOLDER.Match(browserDirRoot.Value);
                if (m.Success) // Overwrite version if possible
                    TraktorVersion = m.Groups[1].Value;
            }

            // Effects, optional (FxSettings.Load may return null)
            // can this move below ?
            FxSettings = FxSettings.Load(xml);

            // Devices
            StringXmlEntry controllerConfigController = xml.GetEntry<DeviceIoConfigController>();
            if (controllerConfigController != null) {
                byte[] decoded = Convert.FromBase64String(controllerConfigController.Value);
                _devicesContainerControllers = new DeviceMappingsContainer(new MemoryStream(decoded));
                var _devices_tmp = _devicesContainerControllers.Devices.List.Select(d => new Device(max_id++, d, RemoveUnusedMIDIDefinitions, false)).ToList();

                // append to whole list
                _devices.AddRange(_devices_tmp);
            }

            StringXmlEntry controllerConfigKeyboard = xml.GetEntry<DeviceIoConfigKeyboard>();
            if (controllerConfigKeyboard != null) {

                byte[] decoded = Convert.FromBase64String(controllerConfigKeyboard.Value);
                _devicesContainerKeyboard = new DeviceMappingsContainer(new MemoryStream(decoded));
                var _devices_tmp = _devicesContainerKeyboard.Devices.List.Select(d => new Device(max_id++, d, RemoveUnusedMIDIDefinitions, true)).ToList();

                // append to whole list
                _devices.AddRange(_devices_tmp);
            }

            load_FX();
        }



        private void load_FX() { 
            // Effects
            var effectSelectorInCommands = getCriticalEffectSelectorInCommands();
            var effectSelectorOutCommands = getCriticalEffectSelectorOutCommands();
            if (effectSelectorInCommands.Any() || effectSelectorOutCommands.Any()) {
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

                // if possible, replace effect indices (position on a list) with ids (actual command)
                if (FxSettings != null)
                    restoreEffectSelectorCommands(effectSelectorInCommands, effectSelectorOutCommands);
                else
                    _ignoreFx = true;
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

            // replace ids (actual FX) with indices (position on a list)
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


            bool optimizeFXList = OptimizeFXList; // how to use  CmdrSettings.Instance.OptimizeFXList ?;
            
            if(optimizeFXList)
                FxSettings = new FxSettings(usedFx, usedSnapshots);
        }

        private void restoreEffectSelectorCommands(IEnumerable<EffectSelectorInCommand> effectSelectorInCommands, IEnumerable<EffectSelectorOutCommand> effectSelectorOutCommands)
        {
            // replace indices (position on a list) with ids (actual FX)
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

        private string getDataAsBase64String(bool isKeyboard)
        {
            byte[] data = null;
            using (MemoryStream stream = new MemoryStream())
            {
                // write DevicesContainer into memory stream
                if (isKeyboard) {
                    _devicesContainerKeyboard.Write(new Writer(stream));
                } else {
                    _devicesContainerControllers.Write(new Writer(stream));
                }

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

            DeviceMappingsContainer container;
            if (device.IsKeyboard) {
                container = _devicesContainerKeyboard;
            } else {
                container = _devicesContainerControllers;
            }

            // todo: simplify this
            if (index == Devices.Count)
            {
                _devices.Add(device);
                container.Devices.List.Add(device.RawDevice);
            }
            else
            {
                _devices.Insert(index, device);
                container.Devices.List.Insert(index, device.RawDevice);
            }
        }
    }
}
