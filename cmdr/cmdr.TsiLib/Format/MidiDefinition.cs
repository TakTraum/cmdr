using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Utils;

namespace cmdr.TsiLib.Format
{
    internal class MidiDefinition : Frame
    {
        public string MidiNote { get; private set; }
        public MidiControlType MidiControlType { get; private set; }
        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }

        // setter must be internal to allow adaptation to different generic midi devices
        public EncoderMode EncoderMode { get; internal set; }

        /// <summary>
        /// In the case of Native Instruments devices seems to identify the 
        /// control Id. However this control Id will be the same for e.g. both 
        /// left and right Shift keys (Kontrol S4). Otherwise 0xFFFFFFFF (-1).
        /// </summary>
        public int ControlId { get; private set; }


        public MidiDefinition(string midiNote, MidiControlType controlType, float minValue, float maxValue, EncoderMode encoderMode, int controlId)
            : base("DCDT")
        {
            MidiNote = midiNote;
            MidiControlType = controlType;
            MinValue = MinValue;
            MaxValue = maxValue;
            EncoderMode = encoderMode;
            ControlId = controlId;
        }

        public MidiDefinition(Stream stream)
            : base(stream)
        {
            MidiNote = stream.ReadWideStringBigE();
            MidiControlType = (MidiControlType)stream.ReadInt32BigE();
            MinValue = stream.ReadFloatBigE();
            MaxValue = stream.ReadFloatBigE();
            EncoderMode = (EncoderMode) stream.ReadInt32BigE();
            ControlId = stream.ReadInt32BigE();
        }


        public override void Write(Writer writer)
        {
            writer.BeginFrame(FrameId);

            writer.WriteWideStringBigE(MidiNote);
            writer.WriteBigE((int)MidiControlType);
            writer.WriteBigE(MinValue);
            writer.WriteBigE(MaxValue);
            writer.WriteBigE((int)EncoderMode);
            writer.WriteBigE(ControlId);

            writer.EndFrame();
        }
    };
}
