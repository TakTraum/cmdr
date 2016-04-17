using System.Collections.Generic;
using System.Runtime.InteropServices;
using cmdr.MidiLib.Core.MidiIO.Definitions;
using cmdr.MidiLib.Core.MidiIO.Exceptions;

namespace cmdr.MidiLib.Core.MidiIO.DeviceInfo
{
    internal sealed class MidiOutInfo : MidiDeviceInfo
    {
        internal MidiOutInfo(
            ushort deviceIndex,
            string productName,
            ushort manufacturerId,
            ushort productId,
            ushort driverVerssion,
            EMidiDeviceTechnology technology,
            ushort voices,
            ushort notes,
            ushort channelMask,
            uint support)
        {
            DeviceIndex = deviceIndex;
            ProductName = productName;
            ManufacturerId = manufacturerId;
            ProductId = productId;
            DriverVersion = driverVerssion;
            Technology = technology;
            Voices = voices;
            Notes = notes;
            ChannelMask = channelMask;
            Support = support;
        }

        public static IEnumerable<MidiOutInfo> Informations
        {
            get
            {
                var retVal = new List<MidiOutInfo>();
                for (ushort i = 0; i < WindowsMultimediaDevice.midiOutGetNumDevs(); i++)
                {
                    var caps = new MidiOutCaps();
                    int error = WindowsMultimediaDevice.midiOutGetDevCaps(i, ref caps, Marshal.SizeOf(caps));
                    if (error != (int)EDeviceException.MmsyserrNoerror) throw new MidiDeviceException(error);
                    retVal.Add(
                        new MidiOutInfo(
                            i,
                            caps.name,
                            (ushort) caps.mid,
                            (ushort) caps.pid,
                            (ushort) caps.driverVersion,
                            (EMidiDeviceTechnology) caps.support,
                            (ushort) caps.voices,
                            (ushort) caps.notes,
                            (ushort) caps.channelMask,
                            (uint) caps.support)
                        );
                }
                return retVal;
            }
        }

        public EMidiDeviceTechnology Technology { get; private set; }

        public ushort Voices { get; private set; }

        public ushort Notes { get; private set; }

        public ushort ChannelMask { get; private set; }

    }
}