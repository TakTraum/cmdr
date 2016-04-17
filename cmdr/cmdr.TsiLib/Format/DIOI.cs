using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using cmdr.TsiLib.Utils;

namespace cmdr.TsiLib.Format
{
    internal class DIOI : Frame
    {
        public int Unknown { get; set; }


        public DIOI()
            : base("DIOI")
        {
            //TODO: Unknown
            Unknown = 1;
        }
        
        public DIOI(Stream stream)
            : base(stream)
        {
            Unknown = stream.ReadInt32BigE();
            if (Unknown != 1)
            {

            }
        }


        public override void Write(Writer writer)
        {
            writer.BeginFrame(FrameId);
            writer.WriteBigE(Unknown);
            writer.EndFrame();
        }
    }
}
