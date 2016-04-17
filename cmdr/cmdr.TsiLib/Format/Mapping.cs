using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Utils;

namespace cmdr.TsiLib.Format
{
    internal class Mapping : Frame
    {
        /// <summary>
        /// Identifier of mapping. Used for midi bindings.
        /// </summary>
        public int MidiNoteBindingId { get; set; }
        public MappingType Type { get; set; }
        public int TraktorControlId { get; set; }
        public MappingSettings Settings { get; set; }


        public Mapping(MappingType type, int traktorControlId)
            : base("CMAI")
        {
            // In Traktor's Controller Manager Ids start at 1. In Xtreme Mapping they start at 0. 
            // For compatibility reasons this value must be initialized with -1;
            MidiNoteBindingId = -1;

            Type = type;
            TraktorControlId = traktorControlId;
            Settings = new MappingSettings();
        }

        public Mapping(Stream stream)
            : base(stream)
        {
            MidiNoteBindingId = stream.ReadInt32BigE();
            Type = (MappingType)stream.ReadInt32BigE();
            TraktorControlId = stream.ReadInt32BigE();
            Settings = new MappingSettings(stream);

            if (Type == MappingType.Out && Settings.ControlType == MappingControlType.Button)
            {
                // TODO: check if this is ok
                Settings.ControlType = MappingControlType.LED;
                Settings.InteractionMode = MappingInteractionMode.Output;
            }
        }


        public override void Write(Writer writer)
        {
            writer.BeginFrame(FrameId);

            writer.WriteBigE(MidiNoteBindingId);
            writer.WriteBigE((int)Type);
            writer.WriteBigE(TraktorControlId);          
            Settings.Write(writer);

            writer.EndFrame();
        }
    }
}
