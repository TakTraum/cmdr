using System;
using cmdr.MidiLib.Enums;

namespace cmdr.MidiLib.Messages
{
    public class MidiPitchBendMessage : MidiMessage
    {
        private int _pitch;
        /// <summary>
        /// Value -8192 - 8191.
        /// </summary>
        public int Pitch
        {
            get { return _pitch; }
            set { _pitch = value; updateLsbAndMsb(); }
        }


        public MidiPitchBendMessage()
            : base(new Core.MidiIO.Data.MidiEvent(new byte[3]), MidiMessageType.PitchBend)
        {
            CoreMessage.AllData[0] = 0xE0;
            Pitch = 0;
        }

        internal MidiPitchBendMessage(Core.MidiIO.Data.MidiEvent ev, MidiMessageType type)
            : base(ev, type)
        {
            int p = ((ev.AllData[2] & 0x7F) << 7) | (ev.AllData[1] & 0x7F);
            Pitch = p - 8192;
        }


        private void updateLsbAndMsb()
        {
            // value must be normed to 0 - 16383, 8192 is center.
            int normValue = Pitch + 8192;

            CoreMessage.AllData[1] = (byte)(normValue & 0x7F);
            CoreMessage.AllData[2] = (byte)((normValue >> 7) & 0x7F);
        }


        public override string ToString()
        {
            return String.Format("{0} - Pitch: {1}", Type, Pitch);
        }
    }
}
