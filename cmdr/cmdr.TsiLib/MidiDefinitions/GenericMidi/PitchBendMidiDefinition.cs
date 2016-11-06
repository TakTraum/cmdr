using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions.Base;

namespace cmdr.TsiLib.MidiDefinitions.GenericMidi
{
    public class PitchBendMidiDefinition : AGenericMidiDefinition
    {
        public PitchBendMidiDefinition(MappingType type, int channel)
            : base(type, channel, "PitchBend", 0f, 16383f)
        {

        }

        internal PitchBendMidiDefinition(MappingType type, int channel, Format.MidiDefinition definiton)
            : base(type, channel, definiton)
        {

        }
    }
}