using System.Collections;

namespace cmdr.MidiLib.Channels
{
    public class MidiChannel
    {
        private readonly int _number;
        public int Number { get { return _number; } }

        private readonly BitArray _bitArray;
        private readonly int _bitIndex;

        public bool Enabled
        {
            get { return _bitArray[_bitIndex]; }
            set { _bitArray[_bitIndex] = value; }
        }


        internal MidiChannel(int number, BitArray bitArray)
        {
            _number = number;
            _bitArray = bitArray;
            _bitIndex = number;
        }
    }
}
