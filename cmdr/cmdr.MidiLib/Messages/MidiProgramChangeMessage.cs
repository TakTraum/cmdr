using System;
using cmdr.MidiLib.Enums;

namespace cmdr.MidiLib.Messages
{
    public class MidiProgramChangeMessage : MidiMessage
    {
        private MidiInstrument _instrument;
        public MidiInstrument Instrument
        {
            get { return _instrument; }
            set { _instrument = value; CoreMessage.AllData[1] = (byte)value; }
        }


        public MidiProgramChangeMessage()
            : base(new Core.MidiIO.Data.MidiEvent(new byte[3]), MidiMessageType.ProgramChange)
        {
            CoreMessage.AllData[0] = 0xC0;
        }

        internal MidiProgramChangeMessage(Core.MidiIO.Data.MidiEvent ev, MidiMessageType type)
            : base(ev, type)
        {
            _instrument = (MidiInstrument) (ev.AllData[1] & 0x7F);
        }


        public override string ToString()
        {
            return String.Format("{0} - Instrument: {1}", Type, Instrument);
        }
    }
}
