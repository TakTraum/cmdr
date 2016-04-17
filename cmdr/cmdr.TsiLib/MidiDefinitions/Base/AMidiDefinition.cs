using System;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions.Proprietary;

namespace cmdr.TsiLib.MidiDefinitions.Base
{
    public abstract class AMidiDefinition
    {
        internal Format.MidiDefinition RawDefinition { get; private set; }
        internal string DeviceTypeStr { get; private set; }

        public MappingType Type { get; private set; }
        public string Note { get { return RawDefinition.MidiNote; } }
        public float MinValue { get { return RawDefinition.MinValue; } }
        public float MaxValue { get { return RawDefinition.MaxValue; } }
        public MidiControlType ControlType { get { return RawDefinition.MidiControlType; } }
       

        internal AMidiDefinition(string deviceTypeStr, MappingType type, Format.MidiDefinition definition)
        {
            DeviceTypeStr = deviceTypeStr;
            Type = type;
            RawDefinition = definition;
        }


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            
            AMidiDefinition other = obj as AMidiDefinition;
            if (other == null)
                throw new ArgumentException();

            return Note.Equals(other.Note) && Type.Equals(other.Type);
        }

        public override int GetHashCode()
        {
            return new { Note, Type }.GetHashCode();
        }


        internal static AMidiDefinition Parse(string deviceTypeStr, MappingType type, Format.MidiDefinition definition)
        {
            // Proprietary
            if (definition.ControlId > -1)
            {
                switch (definition.MidiControlType)
                {
                    case MidiControlType.Button:
                        return new ButtonMidiDefinition(deviceTypeStr, definition);
                    case MidiControlType.FaderOrKnob:
                        return new FaderOrKnobMidiDefinition(deviceTypeStr, definition);
                    case MidiControlType.PushEncoder:
                        return new PushEncoderMidiDefinition(deviceTypeStr, definition);
                    case MidiControlType.Encoder:
                        return new EncoderMidiDefinition(deviceTypeStr, definition);
                    case MidiControlType.Jog:
                        return new JogMidiDefinition(deviceTypeStr, definition);
                    case MidiControlType.Out:
                        return new OutMidiDefinition(deviceTypeStr, definition);
                    default:
                        return null;
                }
            }

            return new GenericMidiDefinition(type, definition);
        }
    }
}
