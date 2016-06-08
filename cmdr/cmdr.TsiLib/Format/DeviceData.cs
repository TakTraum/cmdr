using cmdr.TsiLib.Utils;
using System;
using System.IO;

namespace cmdr.TsiLib.Format
{
    internal class DeviceData : Frame
    {
        public DeviceTargetInfo Target { get; private set; }
        public VersionInfo Version { get; private set; }

        /// <summary>
        /// Is optional.
        /// </summary>
        internal MappingFileComment Comment { get; set; }
        
        public DevicePorts Ports { get; private set; }

        /// <summary>
        /// Is optional.
        /// </summary>
        internal MidiDefinitionsContainer MidiDefinitions { get; set; }

        /// <summary>
        /// Is optional.
        /// </summary>
        internal MappingsContainer Mappings { get; set; }

        /// <summary>
        /// Is optional.
        /// </summary>
        internal DVST Dvst { get; set; }


        public DeviceData(string traktorVersion)
            : base("DDAT")
        {
            Target = new DeviceTargetInfo();
            Version = new VersionInfo(traktorVersion);
            Ports = new DevicePorts();
        }

        public DeviceData(Stream stream)
            : base(stream)
        {
            Target = new DeviceTargetInfo(stream);
            Version = new VersionInfo(stream);

            // look ahead, sometimes comment is skipped
            if (Frame.PeekFourCC(stream) == "DDIC")
                Comment = new MappingFileComment(stream);

            Ports = new DevicePorts(stream);

            // look ahead, sometimes midi definitions are skipped
            if (Frame.PeekFourCC(stream) == "DDDC")
                MidiDefinitions = new MidiDefinitionsContainer(stream);

            // look ahead, sometimes mappings are skipped
            if (Frame.PeekFourCC(stream) == "DDCB")
                Mappings = new MappingsContainer(stream);

            // look ahead, sometimes dvst is skipped
            if (Frame.PeekFourCC(stream) == "DVST")
                Dvst = new DVST(stream);
        }


        public override void Write(Writer writer)
        {
            writer.BeginFrame(FrameId);

            Target.Write(writer);
            
            Version.Write(writer);
            
            if (Comment != null)
                Comment.Write(writer);
            
            Ports.Write(writer);
            
            if (MidiDefinitions != null)
                MidiDefinitions.Write(writer);
            
            if (Mappings != null)
                Mappings.Write(writer);

            if (Dvst != null)
                Dvst.Write(writer);

            writer.EndFrame();
        }
    }
}
