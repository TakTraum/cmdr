using cmdr.TsiLib.Enums;

namespace cmdr.TsiLib.MidiDefinitions.Base
{
    public abstract class AGenericMidiDefinition  :AMidiDefinition
    {
        public AGenericMidiDefinition(MappingType type, string midiNote, float minValue, float maxValue)
            : base(Device.TYPE_STRING_GENERIC_MIDI, type,
            new Format.MidiDefinition(
                midiNote,
                (type == MappingType.In) ? MidiControlType.GenericIn : MidiControlType.Out,
                minValue,
                maxValue,
                MidiEncoderMode.Unknown,
                -1))
        {

        }

        internal AGenericMidiDefinition(MappingType type, Format.MidiDefinition definition)
            : base(Device.TYPE_STRING_GENERIC_MIDI, type, definition)
        {
            
        }
}
}
