using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using cmdr.TsiLib.Utils;

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
