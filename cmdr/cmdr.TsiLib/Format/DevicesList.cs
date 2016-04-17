using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using cmdr.TsiLib.Utils;

namespace cmdr.TsiLib.Format
{
    internal class DevicesList : Frame
    {
        public List<Device> List { get; set; }


        public DevicesList()
            : base("DEVS")
        {
            List = new List<Device>();
        }

        public DevicesList(Stream stream)
            : base(stream)
        {
            List = stream.ReadList<Device>();
        }


        public override void Write(Writer writer)
        {
            writer.BeginFrame(FrameId);
            writer.WriteList<Device>(List);
            writer.EndFrame();
        }
    }
}
