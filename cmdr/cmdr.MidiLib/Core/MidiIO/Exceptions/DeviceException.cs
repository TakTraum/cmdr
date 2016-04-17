using System;

namespace cmdr.MidiLib.Core.MidiIO.Exceptions
{
    internal class DeviceException : ApplicationException
    {
        private readonly int _errorCode;

        public DeviceException(int errorCode)
        {
            _errorCode = errorCode;
        }

        public int ErrorCode
        {
            get
            {
                return _errorCode;
            }
        }
    }
}

