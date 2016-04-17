using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using cmdr.TsiLib.Utils;

namespace cmdr.TsiLib.Format
{
    internal class MappingsList : Frame
    {
        public List<Mapping> Mappings { get; set; }


        public MappingsList()
            : base("CMAS")
        {
            Mappings = new List<Mapping>();
        }

        public MappingsList(Stream stream)
            : base(stream)
        {
            Mappings = stream.ReadList<Mapping>();
        }


        public override void Write(Writer writer)
        {
            writer.BeginFrame(FrameId);
            writer.WriteList<Mapping>(Mappings);
            writer.EndFrame();
        }
    }
}
