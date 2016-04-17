using System.Collections.Generic;
using System.Linq;

namespace cmdr.MidiLib.Messages
{
    public static class MessageFactory
    {
        /// <summary>
        /// cycle through every note on every octave
        /// </summary>
        /// <returns></returns>
        public static List<MidiNoteMessage> AutoDriveNoteOn()
        {
            return Enumerable.Range(0, 128).Select(i => new MidiNoteMessage(true)).ToList();
        }
    }
}
