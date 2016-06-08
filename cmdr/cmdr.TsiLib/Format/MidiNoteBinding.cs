using cmdr.TsiLib.Utils;
using System.IO;

namespace cmdr.TsiLib.Format
{
    internal class MidiNoteBinding : Frame
    {
        public int BindingId { get; set; }
        public string MidiNote { get; set; }


        public MidiNoteBinding(int bindingId, string midiNote)
            : base("DCBM")
        {
            BindingId = bindingId;
            MidiNote = midiNote;
        }

        public MidiNoteBinding(Stream stream)
            : base(stream)
        {
            BindingId = stream.ReadInt32BigE();
            MidiNote = stream.ReadWideStringBigE();
        }


        public override void Write(Writer writer)
        {
            writer.BeginFrame(FrameId);

            writer.WriteBigE(BindingId);
            writer.WriteWideStringBigE(MidiNote);

            writer.EndFrame();
        }
    }
}
