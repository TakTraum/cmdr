using cmdr.TsiLib.Utils;
using System.IO;

namespace cmdr.TsiLib.Format
{
    internal class UnknownFrame : Frame
    {
        public UnknownFrame(Stream stream)
            : base(stream)
        {

        }


        public override void Write(Writer writer)
        {
            if (base.FrameSizeOnDisk.HasValue)
                writer.WriteBigE(new byte[base.FrameSizeOnDisk.Value]);
        }
    }
}
