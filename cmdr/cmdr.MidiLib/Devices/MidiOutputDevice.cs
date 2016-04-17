
using cmdr.MidiLib.IO;
namespace cmdr.MidiLib.Devices
{
    public class MidiOutputDevice : MidiDevice
    {
        internal MidiOutputDevice(int id, string name)
            : base(id, name)
        {

        }


        /// <summary>
        /// Get the output stream for sending messages.
        /// </summary>
        /// <returns></returns>
        public MidiOutputStream GetOutputStream()
        {
            return new MidiOutputStream(new cmdr.MidiLib.Core.MidiIO.OutputDevice(Id));
        }

        /// <summary>
        /// Resets the midi device. Don't call when output stream is opened.
        /// </summary>
        public override void Reset()
        {
            using (var device = new cmdr.MidiLib.Core.MidiIO.OutputDevice(Id))
                device.Reset();
        }
    }
}
