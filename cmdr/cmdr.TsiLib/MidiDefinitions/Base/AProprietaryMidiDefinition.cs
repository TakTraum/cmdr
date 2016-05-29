using cmdr.TsiLib.Enums;

namespace cmdr.TsiLib.MidiDefinitions.Base
{
    public abstract class AProprietaryMidiDefinition : AMidiDefinition
    {
        public int ControlId
        {
            get { return RawDefinition.ControlId; }
        }


        public AProprietaryMidiDefinition(string deviceTypeStr, MappingType type, string midiNote, MidiControlType controlType, float minValue, float maxValue, int controlId)
            : base(deviceTypeStr, type, new Format.MidiDefinition(midiNote, controlType, minValue, maxValue, EncoderMode.Proprietary, controlId))
        {

        }

        internal AProprietaryMidiDefinition(string deviceTypeStr, MappingType type, Format.MidiDefinition definition)
            : base(deviceTypeStr, type, definition)
        {

        }
    }
}
