using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions.Base;

namespace cmdr.TsiLib.MidiDefinitions.Proprietary
{
    public class JogMidiDefinition : AProprietaryMidiDefinition
    {
        public JogMidiDefinition(string deviceTypeStr, string midiNote, int controlId)
            : base(deviceTypeStr, MappingType.In, midiNote, MidiControlType.Jog, -16f, 16f, MidiEncoderMode.Unknown, controlId)
        {

        }

        internal JogMidiDefinition(string deviceTypeStr, Format.MidiDefinition definition)
            : base(deviceTypeStr, MappingType.In, definition)
        {

        }
    }
}
