using System.Collections.Generic;
using System.Linq;
using cmdr.MidiLib.Channels;

namespace cmdr.MidiLib
{
    public static class MidiManager
    {
        private static MidiChannels _channels;
        public static MidiChannels Channels
        {
            get { return _channels ?? (_channels = new MidiChannels()); }
        }

        private static IEnumerable<Devices.MidiInputDevice> _inputDevices;
        public static IEnumerable<Devices.MidiInputDevice> InputDevices { get{return _inputDevices ?? (_inputDevices = getInputDevices());} }

        private static IEnumerable<Devices.MidiInputDevice> _outputDevices;
        public static IEnumerable<Devices.MidiInputDevice> OutputDevices { get {return _outputDevices ?? (_outputDevices = getOutputDevices());} }


        public static void RefreshDevices()
        {
            _inputDevices = getInputDevices();
            _outputDevices = getOutputDevices();
        }


        private static IEnumerable<Devices.MidiInputDevice> getInputDevices()
        {
            return cmdr.MidiLib.Core.MidiIO.DeviceInfo.MidiInInfo.Informations.Select(i => new Devices.MidiInputDevice(i.DeviceIndex, i.ProductName)).ToList();
        }

        private static IEnumerable<Devices.MidiInputDevice> getOutputDevices()
        {
            return cmdr.MidiLib.Core.MidiIO.DeviceInfo.MidiOutInfo.Informations.Select(i => new Devices.MidiInputDevice(i.DeviceIndex, i.ProductName)).ToList();
        }
    }
}
