using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using cmdr.TsiLib.Utils;

namespace cmdr.TsiLib.Format
{
    internal class DevicePorts : Frame
    {
        public string InPortName { get; set; }
        public string OutPortName { get; set; }
        

        public DevicePorts()
            : base("DDPT")
        {
            InPortName = String.Empty;
            OutPortName = String.Empty;
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
