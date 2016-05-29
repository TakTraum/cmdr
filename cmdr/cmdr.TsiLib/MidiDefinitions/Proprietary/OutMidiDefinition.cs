using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions.Base;

namespace cmdr.TsiLib.MidiDefinitions.Proprietary
{
    public class OutMidiDefinition : AProprietaryMidiDefinition
    {
        public OutMidiDefinition(string deviceTypeStr, string midiNote, int controlId)
            : base(deviceTypeStr, MappingType.Out, midiNote, MidiControlType.Out, 0f, 1f, controlId)
        {

        }

        internal OutMidiDefinition(string deviceTypeStr, Format.MidiDefinition definition)
            : base(deviceTypeStr, MappingType.Out, definition)
        {

        }
    }
}
