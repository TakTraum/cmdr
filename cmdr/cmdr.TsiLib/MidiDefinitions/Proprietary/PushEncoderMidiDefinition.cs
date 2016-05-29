using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions.Base;

namespace cmdr.TsiLib.MidiDefinitions.Proprietary
{
    public class PushEncoderMidiDefinition : AProprietaryMidiDefinition
    {
        public PushEncoderMidiDefinition(string deviceTypeStr, string midiNote, float minValue, float maxValue, int controlId)
            : base(deviceTypeStr, MappingType.In, midiNote, MidiControlType.PushEncoder, minValue, maxValue, controlId)
        {

        }

        internal PushEncoderMidiDefinition(string deviceTypeStr, Format.MidiDefinition definition)
            : base(deviceTypeStr, MappingType.In, definition)
        {

        }
    }
}
