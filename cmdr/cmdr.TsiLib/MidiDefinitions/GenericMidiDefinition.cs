using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions.Base;

namespace cmdr.TsiLib.MidiDefinitions
{
    public class GenericMidiDefinition : AGenericMidiDefinition
    {
        public GenericMidiDefinition(MappingType type, string midiNote)
            : base(type, midiNote, 0f,(midiNote.EndsWith("PitchBend") ? 16383f : 127f))
        {

        }

        internal GenericMidiDefinition(MappingType type, Format.MidiDefinition definition)
            : base(type, definition)
        {
            
        }
    }
}
