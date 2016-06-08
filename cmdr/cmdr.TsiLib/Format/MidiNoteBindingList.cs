using cmdr.TsiLib.Utils;
using System.Collections.Generic;
using System.IO;

namespace cmdr.TsiLib.Format
{
    internal class MidiNoteBindingList : Frame
    {
        public List<MidiNoteBinding> Bindings { get; private set; }


        public MidiNoteBindingList()
            : base("DCBM")
        {
            Bindings = new List<MidiNoteBinding>();
        }

        public MidiNoteBindingList(Stream stream)
            : base(stream)
        {
            Bindings = stream.ReadList<MidiNoteBinding>();
        }


        public override void Write(Writer writer)
        {
            writer.BeginFrame(FrameId);

            writer.WriteList(Bindings);
            
            writer.EndFrame();
        }
    };
}
