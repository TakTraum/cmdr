using System;
using cmdr.MidiLib.Enums;

namespace cmdr.MidiLib.Messages
{
    public class MidiNoteMessage : MidiMessage
    {
        private int _key;
        /// <summary>
        /// Key 0 - 127.
        /// </summary>
        public int Key
        {
            get { return _key; }
            set { _key = value; CoreMessage.AllData[1] = (byte)(value & 0x7F); }
        }

        private int _velocity;
        /// <summary>
        /// Velocity 0 - 127.
        /// </summary>
        public int Velocity
        {
            get { return _velocity; }
            set { _velocity = value; CoreMessage.AllData[2] = (byte)(value & 0x7F); }
        }


        public MidiNoteMessage(bool on)
            : base(new Core.MidiIO.Data.MidiEvent(new byte[3]), on ? MidiMessageType.NoteOn: MidiMessageType.NoteOff)
        {
            CoreMessage.AllData[0] = (byte)(on ? 0x90 : 0x80);
        }

        internal MidiNoteMessage(Core.MidiIO.Data.MidiEvent ev, MidiMessageType type)
            : base(ev, type)
        {
            _key = ev.AllData[1] & 0x7F;
            _velocity = ev.AllData[2] & 0x7F;
        }


        public override string ToString()
        {
            return String.Format("{0} - Key: {1} Velocity: {2}", Type, Key, Velocity);
        }
    }
}
