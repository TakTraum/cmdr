using System.Text;
using cmdr.MidiLib.Core.MidiIO.Definitions;

namespace cmdr.MidiLib.Core.MidiIO.Data
{
    internal class MidiEvent
    {
        public MidiEvent(byte[] data)
        {
            AllData = data;
        }

        public readonly byte[] AllData;

        public string Hex
        {
            get
            {
                var sb = new StringBuilder();
                for (int i = 0; i < AllData.Length; i++)
                {
                    sb.Append(AllData[i].ToString("X2").ToUpper());
                }
                return sb.ToString();
            }
        }

        public byte Status { get { return AllData[0]; } }

        /// <summary>
        /// Sets the channel number (1-16).
        /// </summary>
        /// <param name="channelNumber"></param>
        public void SetChannel(int channelNumber)
        {
            byte highNibble = (byte)(AllData[0] & 0xF0); // keep high nibble
            byte lowNibble  = (byte)((channelNumber - 1) & 0x0F);
            AllData[0] = (byte)(highNibble | lowNibble);
        }

        public EMidiEventType MidiEventType
        {
            get
            {
                switch (Status)
                {
                    case 0xFF:
                        return EMidiEventType.Meta;
                    case 0xF0:
                        return EMidiEventType.Sysex;
                    case 0xF7:
                        return EMidiEventType.Sysex;
                    case 0:
                        return EMidiEventType.Empty;
                    default:
                        return EMidiEventType.Short;
                }
            }
        }

    }
}