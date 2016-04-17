using System;
using cmdr.MidiLib.Channels;
using cmdr.MidiLib.Messages;

namespace cmdr.MidiLib.IO
{
    public class MidiOutputStream : IDisposable
    {
        private cmdr.MidiLib.Core.MidiIO.OutputDevice _device;

        internal MidiOutputStream(cmdr.MidiLib.Core.MidiIO.OutputDevice device)
        {
            _device = device;
        }


        public bool Send(MidiChannel channel, MidiMessage message)
        {
            try
            {
                message.CoreMessage.SetChannel(channel.Number);
                _device.Send(message.CoreMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            _device.Dispose();
        }
    }
}
