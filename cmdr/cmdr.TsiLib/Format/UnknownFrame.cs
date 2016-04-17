using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdr.TsiLib.Format
{
    internal class UnknownFrame : Frame
    {
        public UnknownFrame(Stream stream)
            : base(stream)
        {

        }

        public override void Write(Utils.Writer writer)
        {
            if (base.FrameSizeOnDisk.HasValue)
                writer.WriteBigE(new byte[base.FrameSizeOnDisk.Value]);
        }
    }
}
