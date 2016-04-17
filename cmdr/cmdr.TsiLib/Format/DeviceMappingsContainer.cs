using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using cmdr.TsiLib.Utils;

namespace cmdr.TsiLib.Format
{
    internal class DeviceMappingsContainer : Frame
    {
        public DIOI DIOI { get; set; }
        public DevicesList Devices { get; set; }


        public DeviceMappingsContainer()
            : base("DIOM")
        {
            DIOI = new DIOI();
            Devices = new DevicesList();
        }

        public DeviceMappingsContainer(Stream stream)
            : base(stream)
        {
            DIOI = new DIOI(stream);
            Devices = new DevicesList(stream);
        }


        public override void Write(Writer writer)
        {
            writer.BeginFrame(FrameId);

            DIOI.Write(writer);
            Devices.Write(writer);

            writer.EndFrame();
        }
    }
}
