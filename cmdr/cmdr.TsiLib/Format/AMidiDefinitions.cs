using cmdr.TsiLib.Utils;
using System.Collections.Generic;
using System.IO;

namespace cmdr.TsiLib.Format
{
    internal abstract class AMidiDefinitions : Frame
    {
        public List<MidiDefinition> Definitions { get;  set; }


        public AMidiDefinitions(string frameId)
            : base(frameId)
        {
            Definitions = new List<MidiDefinition>();
        }

        public AMidiDefinitions(Stream stream)
            : base(stream)
        {
            Definitions = stream.ReadList<MidiDefinition>();
        }


        public override void Write(Writer writer)
        {
            writer.BeginFrame(FrameId);

            writer.WriteList(Definitions);

            writer.EndFrame();
        }
    }
}
