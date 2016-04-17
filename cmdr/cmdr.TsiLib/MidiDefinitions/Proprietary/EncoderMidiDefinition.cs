using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions.Base;

namespace cmdr.TsiLib.MidiDefinitions.Proprietary
{
    public class EncoderMidiDefinition : AProprietaryMidiDefinition
    {
        public MidiEncoderMode EncoderMode
        {
            get { return RawDefinition.EncoderMode; }
        }


        public EncoderMidiDefinition(string deviceTypeStr, string midiNote, float minValue, float maxValue, MidiEncoderMode encoderMode, int controlId)
            : base(deviceTypeStr, MappingType.In, midiNote, MidiControlType.Encoder, minValue, maxValue, encoderMode, controlId)
        {

        }

        internal EncoderMidiDefinition(string deviceTypeStr, Format.MidiDefinition definition)
            : base(deviceTypeStr, MappingType.In, definition)
        {

        }
    }
}
