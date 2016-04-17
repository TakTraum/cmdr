using cmdr.MidiLib.Enums;

namespace cmdr.MidiLib.Messages
{
    public abstract class MidiMessage
    {
        private readonly cmdr.MidiLib.Core.MidiIO.Data.MidiEvent _coreMessage;
        internal cmdr.MidiLib.Core.MidiIO.Data.MidiEvent CoreMessage { get { return _coreMessage; } }

        private readonly MidiMessageType _type;
        public MidiMessageType Type { get { return _type; } }


        internal MidiMessage(cmdr.MidiLib.Core.MidiIO.Data.MidiEvent ev, MidiMessageType type)
        {
            _coreMessage = ev;
            _type = type;
        }
    }
}
