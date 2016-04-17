using System.Text;

namespace cmdr.MidiLib.Core.MidiIO.Exceptions
{
    internal sealed class OutputDeviceException : MidiDeviceException
    {

        public OutputDeviceException(int errCode) : base(errCode)
        {
            WindowsMultimediaDevice.midiOutGetErrorText(errCode, ErrMsg, ErrMsg.Capacity);
        }

    }
}