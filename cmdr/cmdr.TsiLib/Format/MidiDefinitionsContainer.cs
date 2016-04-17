using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using cmdr.TsiLib.Utils;

namespace cmdr.TsiLib.Format
{
    internal class MidiDefinitionsContainer : Frame
    {
        public MidiInDefinitions In { get; set; }
        public MidiOutDefinitions Out { get; set; }


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

            var filter = In.Definitions.Where(d => d.MidiControlType == Enums.MidiControlType.GenericIn && d.MaxValue != 127f);
            if (filter.Any())
            {

            }

            filter = In.Definitions.Where(d => d.MidiControlType == Enums.MidiControlType.GenericIn && d.MidiNote.Contains("Note"));
            if (filter.Any())
            {

            }

            if (Out.Definitions.Any(d => d.MidiControlType != Enums.MidiControlType.Out))
            {

            }
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
