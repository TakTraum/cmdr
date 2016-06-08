using System.IO;

namespace cmdr.TsiLib.Format
{
    internal class MidiInDefinitions : AMidiDefinitions
    {
        public MidiInDefinitions()
            : base("DDCI")
        {

        }

        public MidiInDefinitions(Stream stream)
            : base(stream)
        {

        }
    }
}
