using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions.Base;
using System;

namespace cmdr.TsiLib.MidiDefinitions.GenericMidi
{
    public class ComboMidiDefinition : AGenericMidiDefinition
    {
        private readonly AGenericMidiDefinition _midiDefinition1;
        public AGenericMidiDefinition MidiDefinition1
        {
            get { return _midiDefinition1; }
        }

        private readonly AGenericMidiDefinition _midiDefinition2;
        public AGenericMidiDefinition MidiDefinition2
        {
            get { return _midiDefinition2; }
        }


        public ComboMidiDefinition(AGenericMidiDefinition def1, AGenericMidiDefinition def2)
            : base(def1.Type, def1.Channel, def1.Note + "+" + def2.Note, def1.MinValue, def1.MaxValue)
        {
            _midiDefinition1 = def1;
            _midiDefinition2 = def2;
        }

        internal ComboMidiDefinition(MappingType type, int channel, Format.MidiDefinition definition)
            : base(type, channel, definition)
        {
            var defs = splitDefinition(definition);
            _midiDefinition1 = AGenericMidiDefinition.Parse(type, defs.Item1);
            _midiDefinition2 = AGenericMidiDefinition.Parse(type, defs.Item2);
        }


        private Tuple<Format.MidiDefinition, Format.MidiDefinition> splitDefinition(Format.MidiDefinition definition)
        {
            var parts = definition.MidiNote.Split('+');
            return new Tuple<Format.MidiDefinition, Format.MidiDefinition>(
                new Format.MidiDefinition(
                                parts[0],
                                definition.MidiControlType,
                                definition.MinValue,
                                definition.MaxValue,
                                definition.EncoderMode,
                                definition.ControlId),
                new Format.MidiDefinition(
                                parts[1],
                                definition.MidiControlType,
                                definition.MinValue,
                                definition.MaxValue,
                                definition.EncoderMode,
                                definition.ControlId)
                                );
        }
    }
}
