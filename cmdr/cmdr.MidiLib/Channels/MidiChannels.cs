using System.Collections;
using System.Linq;

namespace cmdr.MidiLib.Channels
{
    public class MidiChannels
    {
        BitArray _bitArray;
        internal BitArray BitArray
        {
            get { return _bitArray; }
        }

        private MidiChannel[] _channels;


        internal MidiChannels()
        {
            _bitArray = new BitArray(16, true);

            _channels = Enumerable.Range(1, 16)
                .Select(i => new MidiChannel(i, _bitArray)).ToArray();
        }


        /// <summary>
        /// Get Channel by number (1-16).
        /// </summary>
        /// <param name="channelnumber"></param>
        /// <returns></returns>
        public MidiChannel this[int channelnumber]
        {
            get { return _channels[channelnumber - 1]; }
        }

        public void EnableAll()
        {
            _bitArray.SetAll(true);
        }

        public void DisableAll()
        {
            _bitArray.SetAll(false);
        }
    }
}
