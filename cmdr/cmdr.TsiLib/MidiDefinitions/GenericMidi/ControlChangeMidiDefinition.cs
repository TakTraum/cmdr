using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions.Base;

namespace cmdr.TsiLib.MidiDefinitions.GenericMidi
{
    public class ControlChangeMidiDefinition : AGenericMidiDefinition
    {
        private readonly int _cc;
        public int Cc
        {
            get { return _cc; }
        }


        public ControlChangeMidiDefinition(MappingType type, int channel, int cc)
            : base(type, channel, "CC." + string.Format("{0:000}", cc), 0f, 127f)
        {
            _cc = cc;
        }

        internal ControlChangeMidiDefinition(MappingType type, int channel, int cc, Format.MidiDefinition definition)
            : base(type, channel, definition)
        {
            _cc = cc;
        }
    }
}
