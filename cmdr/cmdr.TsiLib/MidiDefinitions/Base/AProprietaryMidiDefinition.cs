using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions.Proprietary;

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


        internal static AProprietaryMidiDefinition Parse(string deviceTypeStr, Format.MidiDefinition definition)
        {
            switch (definition.MidiControlType)
            {
                case MidiControlType.Button:
                    return new ButtonMidiDefinition(deviceTypeStr, definition);
                case MidiControlType.FaderOrKnob:
                    return new FaderOrKnobMidiDefinition(deviceTypeStr, definition);
                case MidiControlType.PushEncoder:
                    return new PushEncoderMidiDefinition(deviceTypeStr, definition);
                case MidiControlType.Encoder:
                    return new EncoderMidiDefinition(deviceTypeStr, definition);
                case MidiControlType.Jog:
                    return new JogMidiDefinition(deviceTypeStr, definition);
                case MidiControlType.Out:
                    return new OutMidiDefinition(deviceTypeStr, definition);
                default:
                    return null;
            }
        }
    }
}
