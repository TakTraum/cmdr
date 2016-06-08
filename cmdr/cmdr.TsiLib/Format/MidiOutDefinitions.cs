using System.IO;

namespace cmdr.TsiLib.Format
{
    internal class MidiOutDefinitions : AMidiDefinitions
    {
        public MidiOutDefinitions()
            : base("DDCO")
        {

        }

        public MidiOutDefinitions(Stream stream)
            : base(stream)
        {

        }
    }
}
