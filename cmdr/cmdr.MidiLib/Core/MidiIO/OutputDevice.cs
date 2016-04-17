using System;
using System.Threading;
using cmdr.MidiLib.Core.MidiIO.Data;
using cmdr.MidiLib.Core.MidiIO.Definitions;
using cmdr.MidiLib.Core.MidiIO.Exceptions;

namespace cmdr.MidiLib.Core.MidiIO
{
    internal sealed class OutputDevice : MidiDevice
    {
        private readonly WindowsMultimediaDevice.MidiProc _midiOutProcIstance;
        private readonly MidiHeaderBuilder _headerBuilder;
        private readonly object _lockObject;
        private int _bufferCount;

        private int _runningStatus;
        private bool _runningStatusEnabled;

        public OutputDevice(int deviceId) : base(deviceId)
        {
            _lockObject = new object();
            _bufferCount = 0;
            _headerBuilder = new MidiHeaderBuilder();
            Handle = 0;
            _runningStatusEnabled = false;
            _runningStatus = 0;
            _midiOutProcIstance = HandleMessage;
            var result = WindowsMultimediaDevice.midiOutOpen(ref Handle, deviceId, _midiOutProcIstance, 0, 0x30000);
            if (result != (int)EDeviceException.MmsyserrNoerror)
            {
                OnError(new ErrorEventArgs(new OutputDeviceException(result)));
            }
        }

        public bool RunningStatusEnabled
        {
            get { return _runningStatusEnabled; }
            set
            {
                _runningStatusEnabled = value;
                _runningStatus = 0;
            }
        }

        public override void Dispose()
        {

            if (!IsDisposed)
            {
                lock (_lockObject)
                {
                    Close();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_lockObject)
                {
                    Reset();
                    int result = WindowsMultimediaDevice.midiOutClose(Handle);
                    if (result != (int)EDeviceException.MmsyserrNoerror)
                    {
                        OnError(new ErrorEventArgs(new OutputDeviceException(result)));
                    }
                }
            }
            else
            {
                WindowsMultimediaDevice.midiOutReset(Handle);
                WindowsMultimediaDevice.midiOutClose(Handle);
            }
            base.Dispose(disposing);
        }

        public override void Close()
        {
            if (!IsDisposed)
            {
                Dispose(true);
            }
        }

        ~OutputDevice()
        {
            Dispose(false);
        }

        private void HandleMessage(int handle, int msg, int instance, int param1, int param2)
        {
            if (((msg != 0x3c7) && (msg != 0x3c8)) && (msg == 0x3c9))
            {
                //DelegateQueue.Post(ReleaseBuffer, new IntPtr(param1));
            }
        }

        public override void Reset()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            _runningStatus = 0;
            lock (_lockObject)
            {
                int result = WindowsMultimediaDevice.midiOutReset(Handle);
                if (result != (int)EDeviceException.MmsyserrNoerror)
                {
                    OnError(new ErrorEventArgs(new OutputDeviceException(result)));
                }
                while (_bufferCount > 0)
                {
                    Monitor.Wait(_lockObject);
                }
            }
        }

        private void Send(int message)
        {
            lock (_lockObject)
            {
                var result = WindowsMultimediaDevice.midiOutShortMsg(Handle, message);
                if (result != (int)EDeviceException.MmsyserrNoerror)
                {
                    OnError(new ErrorEventArgs(new OutputDeviceException(result)));
                }
            }
        }

        private void Send(byte[] message)
        {
            _runningStatus = 0;
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            lock (_lockObject)
            {
                _headerBuilder.InitializeBuffer(message);
                _headerBuilder.Build();
                int result = WindowsMultimediaDevice.midiOutPrepareHeader(Handle, _headerBuilder.Result, SizeOfMidiHeader);
                if (result == (int)EDeviceException.MmsyserrNoerror)
                {
                    _bufferCount++;
                    result = WindowsMultimediaDevice.midiOutLongMsg(Handle, _headerBuilder.Result, SizeOfMidiHeader);
                    if (result != (int)EDeviceException.MmsyserrNoerror)
                    {
                        WindowsMultimediaDevice.midiOutUnprepareHeader(Handle, _headerBuilder.Result, SizeOfMidiHeader);
                        _bufferCount--;
                        _headerBuilder.Destroy();
                        OnError(new ErrorEventArgs(new OutputDeviceException(result)));
                    }
                }
                else
                {
                    _headerBuilder.Destroy();
                    OnError(new ErrorEventArgs(new OutputDeviceException(result)));
                }
            }
        }

        public void Send(MidiEvent message)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (_runningStatusEnabled)
            {
                if (message.MidiEventType == EMidiEventType.Short)
                {
                    int packedMessage = message.AllData[0] + (message.AllData[1] * 0x100) + (message.AllData[2] * 0x10000);
                    if (message.Status == _runningStatus)
                    {
                        Send((packedMessage >> 8));
                    }
                    else
                    {
                        Send(packedMessage);
                        _runningStatus = message.Status;
                    }
                }
                if (message.MidiEventType == EMidiEventType.Sysex)
                {
                    Send(message.AllData);
                }
            }
            else
            {
                if (message.MidiEventType == EMidiEventType.Short)
                {
                    int packedMessage = message.AllData[0] + (message.AllData[1] * 0x100) + (message.AllData[2] * 0x10000);

                    Send(packedMessage);
                }
                if (message.MidiEventType == EMidiEventType.Sysex)
                {
                    Send(message.AllData);
                }
            }
            
        }
    }
}