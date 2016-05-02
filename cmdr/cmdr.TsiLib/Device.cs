using System;
using System.Collections.Generic;
using System.Linq;
using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Format;
using cmdr.TsiLib.MidiDefinitions.Base;

namespace cmdr.TsiLib
{
    public class Device
    {
        public static readonly string TYPE_STRING_GENERIC_MIDI = "Generic MIDI";

        internal Format.Device RawDevice;

        /// <summary>
        /// Internal ID that is only used when added to TsiFile (simple enumeration according to order, not persistent).
        /// Don't forget to set Id in AddDevice method of TsiFile!
        /// </summary>
        public int Id { get; internal set; }

        public int Revision { get { return RawDevice.Data.Version.MappingFileRevision; } }

        public string TypeStr
        {
            get { return RawDevice.DeviceType; }
        }

        public DeviceTarget Target
        {
            get { return RawDevice.Data.Target.DeviceTarget; }
            set { RawDevice.Data.Target.DeviceTarget = value; }
        }

        public string TraktorVersion
        {
            get { return RawDevice.Data.Version.Version; }
            set { RawDevice.Data.Version.Version = value; }
        }

        public string Comment
        {
            get { return (RawDevice.Data.Comment != null) ? RawDevice.Data.Comment.Comment : String.Empty; }
            set
            {
                if (RawDevice.Data.Comment == null)
                    RawDevice.Data.Comment = new Format.MappingFileComment();
                RawDevice.Data.Comment.Comment = value;
            }
        }

        /// <summary>
        /// Name of MIDI input device. Empty String means "All Ports".
        /// </summary>
        public string InPort
        {
            get { return RawDevice.Data.Ports.InPortName; }
            set { RawDevice.Data.Ports.InPortName = value; }
        }

        /// <summary>
        /// Name of MIDI output device. Empty String means "All Ports".
        /// </summary>
        public string OutPort
        {
            get { return RawDevice.Data.Ports.OutPortName; }
            set { RawDevice.Data.Ports.OutPortName = value; }
        }

        private List<Mapping> _mappings = new List<Mapping>();
        public IReadOnlyCollection<Mapping> Mappings { get { return _mappings.AsReadOnly(); } }

        public Proprietary_Controller_DeviceType ProprietaryControllerDeviceType
        {
            get
            {
                if (TypeStr.EndsWith(".Default"))
                    return Proprietary_Controller_DeviceType.Default;
                else
                    return Proprietary_Controller_DeviceType.User;
            }
        }

        private Dictionary<string, AMidiDefinition> _midiInDefinitions = new Dictionary<string, AMidiDefinition>();
        public IReadOnlyDictionary<string, AMidiDefinition> MidiInDefinitions { get { return _midiInDefinitions; } }

        private Dictionary<string, AMidiDefinition> _midiOutDefinitions = new Dictionary<string, AMidiDefinition>();
        public IReadOnlyDictionary<string, AMidiDefinition> MidiOutDefinitions { get { return _midiOutDefinitions; } }



        internal Device(int id, string deviceTypeStr, string traktorVersion)
            : this(id, new Format.Device(deviceTypeStr, traktorVersion))
        {
            // workaround for Xtreme Mapping only: midi definitions must not be null!
            RawDevice.Data.MidiDefinitions = new MidiDefinitionsContainer();
        }

        internal Device(int id, Format.Device rawDevice)
        {
            Id = id;
            RawDevice = rawDevice;

            updateMidiDefinitions();

            if (RawDevice.Data.Mappings != null)
                _mappings = RawDevice.Data.Mappings.List.Mappings.Select(m => new Mapping(this, m)).ToList();
        }


        public Mapping CreateMapping(CommandProxy command)
        {
            return new Mapping(command);
        }

        public void AddMapping(Mapping mapping)
        {
            if (RawDevice.Data.Mappings == null)
                RawDevice.Data.Mappings = new Format.MappingsContainer();

            mapping.Attach(this);

            //mapping.RawMapping.Settings.DeviceType = Type;
            mapping.RawMapping.MidiNoteBindingId = createNewId(); // set correct id no matter if mapping is newly created or not

            // keep midi binding
            if (mapping.MidiBinding != null)
                mapping.SetBinding(this, mapping.MidiBinding);

            RawDevice.Data.Mappings.List.Mappings.Add(mapping.RawMapping);

            _mappings.Add(mapping);
        }

        public void RemoveMapping(int id)
        {
            var old = _mappings.Single(m => m.Id == id);
            _mappings.Remove(old);
            removeMapping(old.RawMapping);
        }

        /// <summary>
        /// Increment MappingFileRevision of device. Int32 Overflow is internally avoided by modulo.
        /// </summary>
        public void IncrementRevision()
        {
            RawDevice.Data.Version.MappingFileRevision = (RawDevice.Data.Version.MappingFileRevision + 1) % int.MaxValue;
        }


        public Device Copy(bool includeMappings)
        {
            Format.Device rawDeviceCopy;
            using (var copyStream = new System.IO.MemoryStream())
            {
                RawDevice.Write(new Utils.Writer(copyStream));
                copyStream.Seek(0, System.IO.SeekOrigin.Begin);
                rawDeviceCopy = new Format.Device(copyStream);
            }

            // reset revision
            rawDeviceCopy.Data.Version.MappingFileRevision = 0;
            if (!includeMappings)
                rawDeviceCopy.Data.Mappings = new MappingsContainer();
            var copy = new Device(-1, rawDeviceCopy);
            return copy;
        }

        /// <summary>
        /// removes a mapping and any corresponding binding
        /// </summary>
        /// <param name="mapping"></param>
        private void removeMapping(Format.Mapping mapping)
        {
            var rawMappings = RawDevice.Data.Mappings.List.Mappings;
            rawMappings.Remove(mapping);
            removeBinding(mapping.MidiNoteBindingId);
        }

        /// <summary>
        /// Removes the given binding and optionally its corresponding definition, if it is not used any more,
        /// </summary>
        /// <param name="bindingId">Id of binding to remove</param>
        /// <param name="definition">Optional. Remove definition if not used by another binding.</param>
        private void removeBinding(int bindingId, AMidiDefinition definition = null)
        {
            var deviceData = RawDevice.Data;
            var bindings = deviceData.Mappings.MidiBindings.Bindings;

            // remove old binding and check if old definition can be removed too, because it is not used in another binding
            var oldBinding = bindings.SingleOrDefault(b => b.BindingId.Equals(bindingId));
            if (oldBinding != null)
            {
                bindings.Remove(oldBinding);

                if (definition != null)
                {
                    var oldBindings = bindings.Where(b => b.MidiNote.Equals(definition.Note));
                    if (!oldBindings.Any())
                        removeDefinition(definition);
                }
            }
        }

        private void removeDefinition(AMidiDefinition definition)
        {
            var deviceData = RawDevice.Data;
            var definitions = (definition.Type == MappingType.In) ? deviceData.MidiDefinitions.In.Definitions : deviceData.MidiDefinitions.Out.Definitions;
            int removedCount = definitions.RemoveAll(d => d.MidiNote.Equals(definition.Note));
            if (removedCount > 0)
            {
                if (definition.Type == MappingType.In)
                    _midiInDefinitions.Remove(definition.Note);
                else
                    _midiOutDefinitions.Remove(definition.Note);
            }
            
            if (removedCount > 1)
            {
                // TODO: Something for the consolidation function
            }
        }

        private int createNewId()
        {
            var mappings = RawDevice.Data.Mappings.List.Mappings;
            int currMaxId = mappings.Any() ? mappings.Max(b => b.MidiNoteBindingId) : 0;
            if (currMaxId < int.MaxValue)
                return currMaxId + 1;
            else
            {
                // search for first unused id
                for (int i = 0; i < int.MaxValue; i++)
                {
                    if (mappings.Any(m => m.MidiNoteBindingId == i))
                        continue;
                    return i;
                }
            }
            throw new Exception("Mappings are full!");
        }

        private void updateMidiDefinitions()
        {
            var definitions = RawDevice.Data.MidiDefinitions;
            if (definitions != null)
            {
                foreach (var inDef in definitions.In.Definitions)
                    _midiInDefinitions[inDef.MidiNote] = AMidiDefinition.Parse(TypeStr, MappingType.In, inDef);

                foreach (var outDef in definitions.Out.Definitions)
                    _midiOutDefinitions[outDef.MidiNote] = AMidiDefinition.Parse(TypeStr, MappingType.Out, outDef);
            }
        }

    }
}
