using cmdr.TsiLib.Enums;

namespace cmdr.TsiLib.MidiDefinitions.Base
{
    public abstract class AGenericMidiDefinition : AMidiDefinition
    {
        internal MidiEncoderMode MidiEncoderMode
        {
            get
            {
                if (RawDefinition.EncoderMode == Enums.EncoderMode._3Fh_41h)
                    return MidiEncoderMode._3Fh_41h;
                return MidiEncoderMode._7Fh_01h;
            }
            set
            {
                if (value == MidiEncoderMode._3Fh_41h)
                    RawDefinition.EncoderMode = Enums.EncoderMode._3Fh_41h;
                else
                    RawDefinition.EncoderMode = Enums.EncoderMode._7Fh_01h;
            }
        }

        public AGenericMidiDefinition(MappingType type, string midiNote, float minValue, float maxValue)
            : base(Device.TYPE_STRING_GENERIC_MIDI, type, new Format.MidiDefinition(
                    midiNote,
                    (type == MappingType.In) ? MidiControlType.GenericIn : MidiControlType.Out,
                    minValue,
                    maxValue,
                    Enums.EncoderMode._3Fh_41h,
                    -1))
        {

        }

        internal AGenericMidiDefinition(MappingType type, Format.MidiDefinition definition)
            : base(Device.TYPE_STRING_GENERIC_MIDI, type, definition)
        {

        }
    }
}
