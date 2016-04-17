using System.Linq;
using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Conditions;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions.Base;

namespace cmdr.TsiLib
{
    public class Mapping
    {
        private readonly Format.Mapping _rawMapping;
        internal Format.Mapping RawMapping { get { return _rawMapping; } }

        public int Id { get { return RawMapping.MidiNoteBindingId; } }

        public ACommand Command { get; private set; }

        public AMidiDefinition MidiBinding { get; private set; }

        public string Comment
        {
            get { return RawMapping.Settings.Comment; }
            set { RawMapping.Settings.Comment = value; }
        }

        private ACondition _condition1;
        public ACondition Condition1
        {
            get { return _condition1; }
            private set
            {
                if (value == null && _condition1 != null)
                    _condition1.Reset();
                _condition1 = value;
            }
        }

        private ACondition _condition2;
        public ACondition Condition2
        {
            get { return _condition2; }
            private set
            {
                if (value == null && _condition2 != null)
                    _condition2.Reset();
                _condition2 = value;
            }
        }

        // only for proprietary controllers and default mappings
        public bool CanOverrideFactoryMap { get; private set; }

        public bool OverrideFactoryMap
        {
            get { return RawMapping.Settings.OverrideFactoryMap; }
            set { RawMapping.Settings.OverrideFactoryMap = value; }
        }


        #region Constructors

        internal Mapping(CommandProxy command)
        {
            _rawMapping = new Format.Mapping(command.MappingType, command.Description.Id);
            Command = command.Create(RawMapping.Settings);
        }

        internal Mapping(Device device, Format.Mapping rawMapping)
            : this(rawMapping)
        {
            Attach(device);

            if (Id >= 0)
            {
                Format.MidiDefinition rawDefinition = getMidiDefinition(device, Command.MappingType, Id);
                if (rawDefinition != null)
                    MidiBinding = AMidiDefinition.Parse(device.TypeStr, Command.MappingType, rawDefinition);
            }
        }

        /// <summary>
        /// Private constructor. Used for copying and as base constructor.
        /// </summary>
        /// <param name="rawMapping"></param>
        private Mapping(Format.Mapping rawMapping)
        {
            _rawMapping = rawMapping;

            bool originalHasValueUI = RawMapping.Settings.HasValueUI;

            Command = Commands.All.GetCommandProxy(rawMapping.TraktorControlId, RawMapping.Type).Create(RawMapping.Settings);
            if (Command == null)
            {

            }

            if (originalHasValueUI != RawMapping.Settings.HasValueUI)
            {
                // TODO: Something for the consolidation function
            }

            Condition1 = (RawMapping.Settings.ConditionOneId > 0) ? Conditions.All.GetConditionProxy(RawMapping.Settings.ConditionOneId).Create(RawMapping.Settings, ConditionNumber.One) : null;
            Condition2 = (RawMapping.Settings.ConditionTwoId > 0) ? Conditions.All.GetConditionProxy(RawMapping.Settings.ConditionTwoId).Create(RawMapping.Settings, ConditionNumber.Two) : null;
        }

        #endregion


        /// <summary>
        /// Set condition.
        /// </summary>
        /// <param name="number">Number of condition</param>
        /// <param name="proxy">ConditionProxy or null to reset condition.</param>
        /// <returns>True if condition was changed.</returns>
        public bool SetCondition(ConditionNumber number, ConditionProxy proxy)
        {
            ACondition condition = (proxy != null) ? proxy.Create(RawMapping.Settings, number) : null;

            bool changed = false;
            if (number == ConditionNumber.One)
            {
                changed = (Condition1 != condition);
                if (changed)
                    Condition1 = condition;
            }
            else
            {
                changed = (Condition1 != condition);
                if (changed)
                    Condition2 = condition;
            }
            return changed;
        }

        /// <summary>
        /// Set binding.
        /// </summary>
        /// <param name="device">The device this mapping belongs to. </param>
        /// <param name="midi">AMidiDefinition or null to reset binding.</param>
        /// <returns>True if binding was changed.</returns>
        public bool SetBinding(Device device, AMidiDefinition midi) // if the Device paramater was left out, it would be possible to make an invalid binding.
        {
            // given device and definition do not match. binding cannot be kept.
            if (midi != null && device.TypeStr != midi.DeviceTypeStr)
            {
                MidiBinding = null;
                return true;
            }

            // midi definitions and bindings
            var deviceData = device.RawDevice.Data;
            var definitions = (Command.MappingType == MappingType.In) ? deviceData.MidiDefinitions.In.Definitions : deviceData.MidiDefinitions.Out.Definitions;
            var bindings = deviceData.Mappings.MidiBindings.Bindings;

            // remove old binding and definition(s) if no longer in use
            if (MidiBinding != null)
            {
                // remove old binding and check if old definition can be removed too, because it is not used in another binding
                var oldBinding = bindings.SingleOrDefault(b => b.BindingId.Equals(Id));
                if (oldBinding != null)
                {
                    bindings.Remove(oldBinding);

                    var oldBindings = bindings.Where(b => b.MidiNote.Equals(MidiBinding.Note));
                    if (!oldBindings.Any())
                    {
                        int removedCount = definitions.RemoveAll(d => d.MidiNote.Equals(MidiBinding.Note));
                        if (removedCount > 1)
                        {
                            // TODO: Something for the consolidation function
                        }
                    }
                }
            }

            if (midi != null)
            {
                // add midi definition to device, if it doesn't already exist
                var matchingDefinitions = definitions.Where(d => d.MidiNote.Equals(midi.Note));
                if (!matchingDefinitions.Any())
                    definitions.Add(midi.RawDefinition);

                // add midi binding to device
                bindings.Add(new Format.MidiNoteBinding(Id, midi.Note));
            }

            // set midi binding to mapping
            bool changed = (MidiBinding != null) ? !MidiBinding.Equals(midi) : ((midi != null) ? !midi.Equals(MidiBinding) : false);
            if (changed)
                MidiBinding = midi;

            return changed;
        }

        /// <summary>
        /// Creates a copy of this mapping.
        /// </summary>
        /// <param name="includeMidiBinding">Optionally include the midi binding.</param>
        /// <returns>Copy of mapping</returns>
        public Mapping Copy(bool includeMidiBinding)
        {
            Format.Mapping rawMappingCopy;
            using (var copyStream = new System.IO.MemoryStream())
            {
                RawMapping.Write(new Utils.Writer(copyStream));
                copyStream.Seek(0, System.IO.SeekOrigin.Begin);
                rawMappingCopy = new Format.Mapping(copyStream);
            }

            var copy = new Mapping(rawMappingCopy);
            if (includeMidiBinding)
                copy.MidiBinding = MidiBinding;

            return copy;
        }


        internal void Attach(Device device)
        {
            CanOverrideFactoryMap = (device.TypeStr != Device.TYPE_STRING_GENERIC_MIDI && device.ProprietaryControllerDeviceType == Proprietary_Controller_DeviceType.Default);
        }


        private Format.MidiDefinition getMidiDefinition(Device device, MappingType type, int id)
        {
            var deviceData = device.RawDevice.Data;
            var definitions = (Command.MappingType == MappingType.In) ? deviceData.MidiDefinitions.In.Definitions : deviceData.MidiDefinitions.Out.Definitions;
            var bindings = deviceData.Mappings.MidiBindings.Bindings;

            var rawBinding = bindings.SingleOrDefault(b => b.BindingId.Equals(id));
            if (rawBinding != null)
            {
                var defs = definitions.Where(d => d.MidiNote.Equals(rawBinding.MidiNote));
                if (defs.Any())
                {
                    if (defs.Count() == 1)
                        return defs.Single();
                    else // definition duplicates found
                    {
                        // this should not happen actually, but there seems to be a bug in 
                        // Xtreme Mapping / Traktor Controller Manager.
                        // TODO: Something for the consolidation function
                        return defs.First();
                    }
                }
            }
            return null;
        }
    }
}
