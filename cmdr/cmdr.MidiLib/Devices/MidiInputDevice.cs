
using cmdr.MidiLib.IO;
namespace cmdr.MidiLib.Devices
{
    public class MidiInputDevice : MidiDevice
    {
        private MidiMessageBroker _messageBroker = null;

        internal MidiInputDevice(int id, string name)
            : base(id, name)
        {

        }


        public MidiMessageBroker GetMessageBroker()
        {
            if (_messageBroker == null)
                _messageBroker = new MidiMessageBroker(this, MidiManager.Channels.BitArray);
            return _messageBroker;
        }

        /// <summary>
        /// Resets the midi device. Don't call when message broker is running.
        /// </summary>
        public override void Reset()
        {
            using (var device = new cmdr.MidiLib.Core.MidiIO.InputDevice(Id))
                device.Reset();
        }
    }
}
