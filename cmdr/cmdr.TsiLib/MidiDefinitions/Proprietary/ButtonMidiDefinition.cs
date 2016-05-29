using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions.Base;

namespace cmdr.TsiLib.MidiDefinitions.Proprietary
{
    public class ButtonMidiDefinition : AProprietaryMidiDefinition
    {
        public ButtonMidiDefinition(string deviceTypeStr, string midiNote, float minValue, float maxValue, int controlId)
            : base(deviceTypeStr, MappingType.In, midiNote, MidiControlType.Button, minValue, maxValue, controlId)
        {

        }

        internal ButtonMidiDefinition(string deviceTypeStr, Format.MidiDefinition definition)
            : base(deviceTypeStr, MappingType.In, definition)
        {

        }
    }
}
