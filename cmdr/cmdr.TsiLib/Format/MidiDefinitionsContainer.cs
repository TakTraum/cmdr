using cmdr.TsiLib.Utils;
using System.IO;

namespace cmdr.TsiLib.Format
{
    internal class MidiDefinitionsContainer : Frame
    {
        public MidiInDefinitions In { get; private set; }
        public MidiOutDefinitions Out { get; private set; }


        public MidiDefinitionsContainer()
            : base("DDDC")
        {
            In = new MidiInDefinitions();
            Out = new MidiOutDefinitions();
        }

        public MidiDefinitionsContainer(Stream stream)
            : base(stream)
        {
            In = new MidiInDefinitions(stream);
            Out = new MidiOutDefinitions(stream);
        }


        public override void Write(Writer writer)
        {
            writer.BeginFrame(FrameId);

            In.Write(writer);
            Out.Write(writer);

            writer.EndFrame();
        }
    }
}
