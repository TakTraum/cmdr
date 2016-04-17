using System.Text;

namespace cmdr.MidiLib.Core.MidiIO.Exceptions
{
    internal sealed class InputDeviceException : MidiDeviceException
    {
       
        public InputDeviceException(int errCode) : base(errCode)
        {
            WindowsMultimediaDevice.midiInGetErrorText(errCode, ErrMsg, ErrMsg.Capacity);
        }       
        
    }
}

