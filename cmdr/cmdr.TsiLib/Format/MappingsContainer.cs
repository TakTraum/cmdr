using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using cmdr.TsiLib.Utils;

namespace cmdr.TsiLib.Format
{
    internal class MappingsContainer : Frame
    {
        public MappingsList List { get; set; }
        public MidiNoteBindingList MidiBindings { get; set; }


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
