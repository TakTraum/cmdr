using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using cmdr.TsiLib.Utils;

namespace cmdr.TsiLib.Format
{
    internal class MappingFileComment  : Frame
    {
        public string Comment { get; set; }


        public MappingFileComment()
            : base("DDIC")
        {
            Comment = String.Empty;
        }

        public MappingFileComment(Stream stream)
            : base(stream)
        {
            Comment = stream.ReadWideStringBigE();
        }


        public override void Write(Writer writer)
        {
            writer.BeginFrame(FrameId);
            writer.WriteWideStringBigE(Comment);
            writer.EndFrame();
        }
    }
}
