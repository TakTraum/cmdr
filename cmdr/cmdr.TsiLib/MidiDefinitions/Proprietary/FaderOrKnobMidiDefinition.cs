using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions.Base;

namespace cmdr.TsiLib.MidiDefinitions.Proprietary
{
    public class FaderOrKnobMidiDefinition : AProprietaryMidiDefinition
    {
        public FaderOrKnobMidiDefinition(string deviceTypeStr, string midiNote, int controlId)
            : base(deviceTypeStr, MappingType.In, midiNote, MidiControlType.FaderOrKnob, 0f, 1f, MidiEncoderMode.Unknown, controlId)
        {

        }

        internal FaderOrKnobMidiDefinition(string deviceTypeStr, Format.MidiDefinition definition)
            : base(deviceTypeStr, MappingType.In, definition)
        {

        }
    }
}
