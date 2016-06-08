using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Utils;
using System.IO;

namespace cmdr.TsiLib.Format
{
    internal class DeviceTargetInfo : Frame
    {
        public DeviceTarget DeviceTarget { get; set; }


        public DeviceTargetInfo()
            : base("DDIF")
        {

        }

        public DeviceTargetInfo(Stream stream)
            : base(stream)
        {
            DeviceTarget = (DeviceTarget) stream.ReadInt32BigE();
        }


        public override void Write(Writer writer)
        {
            writer.BeginFrame(FrameId);

            writer.WriteBigE((int)DeviceTarget);

            writer.EndFrame();
        }
    }
}
