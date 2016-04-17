using System;
using System.Runtime.InteropServices;
using cmdr.MidiLib.Core.MidiIO.Data;
using cmdr.MidiLib.Core.MidiIO.Exceptions;

namespace cmdr.MidiLib.Core.MidiIO
{
    internal sealed class InputDevice : MidiDevice
    {
        #region Delegates

        #endregion

        private readonly MidiHeaderBuilder _headerBuilder;
        private readonly object _lockObject;

        private readonly WindowsMultimediaDevice.MidiProc _midiInProc;
        private volatile int _bufferCount;
        private bool _recording;
        private volatile bool _resetting;
        private int _sysExBufferSize;

        public InputDevice(int deviceId) : base(deviceId)
        {
            _bufferCount = 0;
            _lockObject = new object();
            _recording = false;
            _headerBuilder = new MidiHeaderBuilder();
            Handle = 0;
            _resetting = false;
            _sysExBufferSize = 0x1000;
            _midiInProc = HandleMessage;
            var result = WindowsMultimediaDevice.midiInOpen(ref Handle, deviceId, _midiInProc, 0, 0x30000);
            if (result != (int)EDeviceException.MmsyserrNoerror)
            {
                throw new InputDeviceException(result);
            }
        }

        public int SysExBufferSize
        {
            get { return _sysExBufferSize; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _sysExBufferSize = value;
            }
        }

        public MidiMessageEvent OnMidiEvent { get; set; }
        
        public int AddSysExBuffer()
        {
            _headerBuilder.BufferLength = _sysExBufferSize;
            _headerBuilder.Build();
            var headerPtr = _headerBuilder.Result;
            var result = WindowsMultimediaDevice.midiInPrepareHeader(Handle, headerPtr, SizeOfMidiHeader);
            if (result == 0)
            {
                _bufferCount++;
                result = WindowsMultimediaDevice.midiInAddBuffer(Handle, headerPtr, SizeOfMidiHeader);
                if (result != 0)
                {
                    WindowsMultimediaDevice.midiInUnprepareHeader(Handle, headerPtr, SizeOfMidiHeader);
                    _bufferCount--;
                    _headerBuilder.Destroy();
                }
                return result;
            }
            _headerBuilder.Destroy();
            return result;
        }

        public override void Close()
        {
            if (!IsDisposed)
            {
                Dispose(true);
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_lockObject)
                {
                    Reset();
                    
                    var result = WindowsMultimediaDevice.midiInClose(Handle);
                    if (result != 0)
                    {
                        throw new InputDeviceException(result);
                    }
                }
            }
            else
            {
                WindowsMultimediaDevice.midiInReset(Handle);
                WindowsMultimediaDevice.midiInClose(Handle);
            }
            base.Dispose(disposing);
        }

        ~InputDevice()
        {
            if (!IsDisposed)
            {
                WindowsMultimediaDevice.midiInClose(Handle);
            }
        }

        private void HandleMessage(int handle, int msg, int instance, int param1, int param2)
        {
            if ((msg != 0x3c1) && (msg != 0x3c2))
            {
                if (msg == 0x3c3)
                {
                    HandleShortMessage(param1);
                }
                else if (msg == 0x3cc)
                {
                    HandleShortMessage(param1);
                }
                else if (msg == 0x3c4)
                {
                    HandleSysExMessage(new IntPtr(param1), true);
                }
                else if (msg == 0x3c6)
                {
                    HandleSysExMessage(new IntPtr(param1), true);
                }
            }
        }

        private void HandleShortMessage(int packedMessage)
        {
            var msg = new byte[3];
            
            msg[0] = (byte) (packedMessage & 0xff);
            msg[1] = (byte) ((packedMessage & 0xff00) >> 8);
            msg[2] = (byte) (((packedMessage & -65536) >> 0x10));
            
            if (OnMidiEvent != null) OnMidiEvent(new MidiEvent(msg));
        }

        private void HandleSysExMessage(IntPtr sysexPointer, bool isValidSysex)
        {
            byte[] data = null;
            var dataAssigned = false;
            lock (_lockObject)
            {
                var header = (MidiHeader) Marshal.PtrToStructure(sysexPointer, typeof (MidiHeader));
                if (!_resetting)
                {
                    data = new byte[header.bytesRecorded];
                    Marshal.Copy(header.data, data, 0, data.Length);
                    dataAssigned = true;
                    var result = AddSysExBuffer();
                    if (result != 0)
                    {
                        Exception ex = new InputDeviceException(result);
                        OnError(new ErrorEventArgs(ex));
                    }
                }
                ReleaseBuffer(sysexPointer);
            }
            if (!dataAssigned) return;
            if (!isValidSysex) return;
            if (OnMidiEvent != null) OnMidiEvent(new MidiEvent(data));
        }

        private void ReleaseBuffer(IntPtr headerPtr)
        {
            int result = WindowsMultimediaDevice.midiInUnprepareHeader(Handle, headerPtr, SizeOfMidiHeader);
            if (result != (int)EDeviceException.MmsyserrNoerror)
            {
                OnError(new ErrorEventArgs(new InputDeviceException(result)));
            }
            _headerBuilder.Destroy(headerPtr);
            _bufferCount--;
        }

        public override void Reset()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("InputDevice");
            }
            lock (_lockObject)
            {
                _resetting = true;
                var result = WindowsMultimediaDevice.midiInReset(Handle);
                if (result == (int)EDeviceException.MmsyserrNoerror)
                {
                    _recording = false;                    
                    _resetting = false;
                }
                else
                {
                    _resetting = false;
                    throw new InputDeviceException(result);
                }
            }
        }

        public void Start()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("InputDevice");
            }
            if (!_recording)
            {
                lock (_lockObject)
                {
                    var result = 0;
                    const int bufcount = 4;
                    var currentBufCount = 0;
                    while (result == (int)EDeviceException.MmsyserrNoerror && currentBufCount < bufcount)
                    {
                        result = AddSysExBuffer();
                        currentBufCount++;
                    }
                    if (result == (int)EDeviceException.MmsyserrNoerror)
                    {
                        result = WindowsMultimediaDevice.midiInStart(Handle);
                    }
                    if (result != (int)EDeviceException.MmsyserrNoerror)
                    {
                        throw new InputDeviceException(result);
                    }
                    _recording = true;
                }
            }

        }

        public void Stop()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("InputDevice");
            }
            if (_recording)
            {
                lock (_lockObject)
                {
                    int result = WindowsMultimediaDevice.midiInStop(Handle);
                    if (result != 0)
                    {
                        throw new InputDeviceException(result);
                    }
                    _recording = false;
                }
            }
        }
    }
}