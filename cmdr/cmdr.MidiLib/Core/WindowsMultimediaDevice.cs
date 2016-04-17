using System;
using System.Runtime.InteropServices;
using System.Text;
using cmdr.MidiLib.Core.MidiIO.DeviceInfo;

namespace cmdr.MidiLib.Core
{
    /// <summary>
    /// Base class for methods stored in winmm.dll.
    /// </summary>
    internal static class WindowsMultimediaDevice
    {
        [DllImport("winmm.dll")]
        internal static extern int midiConnect(int handleA, int handleB, int reserved);

        [DllImport("winmm.dll")]
        internal static extern int midiDisconnect(int handleA, int handleB, int reserved);

        [DllImport("winmm.dll")]
        internal static extern int midiOutClose(int handle);

        [DllImport("winmm.dll")]
        internal static extern int midiOutOpen(ref int handle, int deviceId, MidiProc proc, int instance, int flags);

        [DllImport("winmm.dll")]
        internal static extern int midiInAddBuffer(int handle, IntPtr headerPtr, int sizeOfMidiHeader);

        [DllImport("winmm.dll")]
        internal static extern int midiInClose(int handle);

        [DllImport("winmm.dll")]
        internal static extern int midiInGetDevCaps(int deviceId, ref MidiInCaps caps, int sizeOfMidiInCaps);

        [DllImport("winmm.dll")]
        internal static extern int midiInGetNumDevs();

        [DllImport("winmm.dll")]
        internal static extern int midiInOpen(ref int handle, int deviceId, MidiProc proc, int instance, int flags);

        [DllImport("winmm.dll")]
        internal static extern int midiInPrepareHeader(int handle, IntPtr headerPtr, int sizeOfMidiHeader);

        [DllImport("winmm.dll")]
        internal static extern int midiInReset(int handle);

        [DllImport("winmm.dll")]
        internal static extern int midiInStart(int handle);

        [DllImport("winmm.dll")]
        internal static extern int midiInStop(int handle);

        [DllImport("winmm.dll")]
        internal static extern int midiInUnprepareHeader(int handle, IntPtr headerPtr, int sizeOfMidiHeader);

        [DllImport("winmm.dll")]
        internal static extern int midiOutGetDevCaps(int deviceId, ref MidiOutCaps caps, int sizeOfMidiOutCaps);

        [DllImport("winmm.dll")]
        internal static extern int midiOutGetNumDevs();

        [DllImport("winmm.dll")]
        internal static extern int midiOutLongMsg(int handle, IntPtr headerPtr, int sizeOfMidiHeader);

        [DllImport("winmm.dll")]
        internal static extern int midiOutPrepareHeader(int handle, IntPtr headerPtr, int sizeOfMidiHeader);

        [DllImport("winmm.dll")]
        internal static extern int midiInGetErrorText(int errCode, StringBuilder errMsg, int sizeOfErrMsg);

        [DllImport("winmm.dll")]
        internal static extern int midiOutGetErrorText(int errCode, StringBuilder message, int sizeOfMessage);

        [DllImport("winmm.dll")]
        internal static extern int midiOutReset(int handle);

        [DllImport("winmm.dll")]
        internal static extern int midiOutShortMsg(int handle, int message);

        [DllImport("winmm.dll")]
        internal static extern int midiOutUnprepareHeader(int handle, IntPtr headerPtr, int sizeOfMidiHeader);

        #region Nested type: MidiProc

        internal delegate void MidiProc(int handle, int msg, int instance, int param1, int param2);

        #endregion

    }
}