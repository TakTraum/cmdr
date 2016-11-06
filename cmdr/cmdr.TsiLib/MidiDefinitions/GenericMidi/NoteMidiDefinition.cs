using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions.Base;

namespace cmdr.TsiLib.MidiDefinitions.GenericMidi
{
    public class NoteMidiDefinition : AGenericMidiDefinition
    {
        private readonly string _keyText;
        public string KeyText
        {
            get { return _keyText; }
        } 


        public NoteMidiDefinition(MappingType type, int channel, string keyText)
            : base(type, channel, "Note." + keyText, 0f, 127f)
        {
            _keyText = keyText;
        }

        internal NoteMidiDefinition(MappingType type, int channel, string keyText, Format.MidiDefinition definition)
            : base(type, channel, definition)
        {
            _keyText = keyText;
        }
    }
}
