using System;
using cmdr.TsiLib.Enums;

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
            return new { Id = Note, Type }.GetHashCode();
        }


        internal static AMidiDefinition Parse(string deviceTypeStr, MappingType type, Format.MidiDefinition definition)
        {

            if (
                (deviceTypeStr == "Traktor.Kontrol S4 MK3") ||
                (deviceTypeStr == "Traktor.Kontrol S2 MK3") ||
                (deviceTypeStr == "Traktor.Kontrol S8") ||
                false
                )
                return AProprietaryMidiDefinition.Parse(deviceTypeStr, definition);

            if (definition.ControlId > -1)
                return AProprietaryMidiDefinition.Parse(deviceTypeStr, definition);
            else
                return AGenericMidiDefinition.Parse(type, definition);
        }
    }
}
