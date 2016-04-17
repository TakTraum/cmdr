using System;
using cmdr.MidiLib.Enums;

namespace cmdr.MidiLib.Messages
{
    public class MidiControlChangeMessage : MidiMessage
    {
        private int _controlId;
        /// <summary>
        /// Id of control (fader, knob).
        /// </summary>
        public int ControlId
        {
            get { return _controlId; }
            set { _controlId = value; CoreMessage.AllData[1] = (byte)value; }
        }

        private int _controlValue;
        /// <summary>
        /// Control value.
        /// </summary>
        public int ControlValue
        {
            get { return _controlValue; }
            set { _controlValue = value; CoreMessage.AllData[2] = (byte)value; }
        }


        public MidiControlChangeMessage()
            : base(new Core.MidiIO.Data.MidiEvent(new byte[3]), MidiMessageType.ControlChange)
        {
            CoreMessage.AllData[0] = 0xB0;
        }

        internal MidiControlChangeMessage(Core.MidiIO.Data.MidiEvent ev, MidiMessageType type)
            : base(ev, type)
        {
            ControlId = ev.AllData[1];
            ControlValue = ev.AllData[2];
        }


        public override string ToString()
        {
            return String.Format("{0} - ControlId: {1} ControlValue: {2}", Type, ControlId, ControlValue);
        }
    }
}
