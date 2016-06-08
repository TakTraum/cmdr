using cmdr.TsiLib.Utils;
using System.IO;

namespace cmdr.TsiLib.Format
{
    internal class VersionInfo : Frame
    {
        public string Version { get; set; }

        public int MappingFileRevision { get; set; }
        

        public VersionInfo(string version)
            : base("DDIV")
        {
            Version = version;
        }
        
        public VersionInfo(Stream stream)
            : base(stream)
        {
            Version = stream.ReadWideStringBigE();
            MappingFileRevision = stream.ReadInt32BigE();
        }


        public override void Write(Writer writer)
        {
            writer.BeginFrame(FrameId);

            writer.WriteWideStringBigE(Version);
            writer.WriteBigE(MappingFileRevision);

            writer.EndFrame();
        }
    }
}
