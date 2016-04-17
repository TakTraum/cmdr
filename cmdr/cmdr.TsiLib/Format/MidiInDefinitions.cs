using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using cmdr.TsiLib.Utils;

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
