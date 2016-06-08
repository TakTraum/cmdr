using cmdr.TsiLib.Utils;
using System.IO;

namespace cmdr.TsiLib.Format
{
    internal class DeviceMappingsContainer : Frame
    {
        public DIOI DIOI { get; private set; }
        public DevicesList Devices { get; private set; }


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
