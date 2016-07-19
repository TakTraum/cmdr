using cmdr.TsiLib.Utils;
using System;
using System.IO;

namespace cmdr.TsiLib.Format
{
    internal class DevicePorts : Frame
    {
        public static readonly string DEFAULT_PORT = "None";

        public string InPortName { get; set; }
        public string OutPortName { get; set; }
        

        public DevicePorts()
            : base("DDPT")
        {
            InPortName = DEFAULT_PORT;
            OutPortName = DEFAULT_PORT;
        }

        public DevicePorts(Stream stream)
            : base(stream)
        {
            InPortName = stream.ReadWideStringBigE();
            OutPortName = stream.ReadWideStringBigE();
        }


        public override void Write(Writer writer)
        {
            writer.BeginFrame(FrameId);

            writer.WriteWideStringBigE(InPortName);
            writer.WriteWideStringBigE(OutPortName);

            writer.EndFrame();
        }
    }
}
