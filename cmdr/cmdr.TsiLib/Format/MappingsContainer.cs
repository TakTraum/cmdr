using cmdr.TsiLib.Utils;
using System.IO;

namespace cmdr.TsiLib.Format
{
    internal class MappingsContainer : Frame
    {
        public MappingsList List { get; private set; }
        public MidiNoteBindingList MidiBindings { get; private set; }


        public MappingsContainer()
            : base("DDCB")
        {
            List = new MappingsList();
            MidiBindings = new MidiNoteBindingList();
        }

        public MappingsContainer(Stream stream)
            : base(stream)
        {
            List = new MappingsList(stream);
            MidiBindings = new MidiNoteBindingList(stream);
        }


        public override void Write(Writer writer)
        {
            writer.BeginFrame(FrameId);

            List.Write(writer);
            MidiBindings.Write(writer);

            writer.EndFrame();
        }
    }
}
