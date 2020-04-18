using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions.GenericMidi;

namespace cmdr.TsiLib.MidiDefinitions.Base
{
    public abstract class AGenericMidiDefinition : AMidiDefinition
    {
        private readonly int _channel;
        public int Channel
        {
            get { return _channel; }
        }

        private readonly string _note;
        public string Note
        {
            get { return _note; }
        }

        public MidiEncoderMode MidiEncoderMode
        {
            get
            {
                if (RawDefinition.EncoderMode == Enums.EncoderMode._3Fh_41h)
                    return MidiEncoderMode._3Fh_41h;
                return MidiEncoderMode._7Fh_01h;
            }
            set
            {
                if (value == MidiEncoderMode._3Fh_41h)
                    RawDefinition.EncoderMode = Enums.EncoderMode._3Fh_41h;
                else
                    RawDefinition.EncoderMode = Enums.EncoderMode._7Fh_01h;
            }
        }


         public AGenericMidiDefinition(MappingType type, int channel, string note, float minValue, float maxValue)
            : base(Device.TYPE_STRING_GENERIC_MIDI, type, new Format.MidiDefinition(
                    string.Format("Ch{0:00}.{1}", channel, note.Contains("+") ? note.Replace("+", string.Format("+Ch{0:00}.", channel)) : note),
                    (type == MappingType.In) ? MidiControlType.GenericIn : MidiControlType.Out,
                    minValue,
                    maxValue,
                    Enums.EncoderMode._3Fh_41h,
                    -1))
        {
            _channel = channel;
            _note = note;
        }

        internal AGenericMidiDefinition(MappingType type, int channel, Format.MidiDefinition definition)
            : base(Device.TYPE_STRING_GENERIC_MIDI, type, definition)
        {
            _channel = channel;
            _note = definition.MidiNote.Replace(string.Format("Ch{0:00}.", channel), "");
        }


        public static AGenericMidiDefinition Parse(MappingType type, string id)
        {
            if (id.Contains("+"))
            {
                var combo = id.Split('+');
                return new ComboMidiDefinition(Parse(type, combo[0]), Parse(type, combo[1]));
            }

            int channel = int.Parse(id.Substring(2, 2));
            var parts = id.Split('.');
            switch (parts[1])
            {
                case "CC":
                    return new ControlChangeMidiDefinition(type, channel, int.Parse(parts[2]));
                case "Note":
                    return new NoteMidiDefinition(type, channel, parts[2]);
                case "PitchBend":
                    return new PitchBendMidiDefinition(type, channel);
                default:
                    return null;
            }
        }

        internal static AGenericMidiDefinition Parse(MappingType type, Format.MidiDefinition definition)
        {
            string id = definition.MidiNote;
            if (string.IsNullOrEmpty(id))
                return null;

            int channel = int.Parse(id.Substring(2, 2));
            if (id.Contains("+"))
                return new ComboMidiDefinition(type, channel, definition);

            var parts = id.Split('.');
            switch (parts[1])
            {
                case "CC":
                    return new ControlChangeMidiDefinition(type, channel, int.Parse(parts[2]), definition);
                case "Note":
                    return new NoteMidiDefinition(type, channel, parts[2], definition);
                case "PitchBend":
                    return new PitchBendMidiDefinition(type, channel, definition);
                default:
                    return null;
            }
        }
    }
}
