using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Utils;
using System;
using System.IO;

namespace cmdr.TsiLib.Format
{
    internal class MappingSettings : Frame
    {
        // set by device when adding a new mapping
        public DeviceType DeviceType { get; set; }

        // set by command
        public MappingControlType ControlType { get; set; } 
        public MappingInteractionMode InteractionMode { get; set; }
        public MappingTargetDeck Target { get; set; }
        public bool AutoRepeat { get; set; }
        public bool Invert { get; set; }
        public bool SoftTakeover { get; set; }

        /// <summary>
        /// 1% in the Traktor UI corresponds to 0.5f Traktor sets this to 
        /// 300% / 15f when Interaction mode is Direct
        /// </summary>
        public float RotarySensitivity { get; set; }

        /// <summary>
        /// Percentage Value between 0.0 and 1.0.
        /// </summary>
        public float RotaryAcceleration { get; set; }

        public MidiEncoderMode EncoderMode2
        {
            get;
            set;
        }

        public bool HasValueUI { get; set; }
        public ValueUIType ValueUIType { get; set; }
        public byte[] SetValueTo { get; set; }
        public string Comment { get; set; }

        // Condition Ids correspond to Command Ids
        // set by ACondition and AValueCondition
        public int ConditionOneId { get; set; }
        public MappingTargetDeck ConditionOneTarget { get; set; } 
        public byte[] ConditionOneValue { get; set; }
        public int ConditionTwoId { get; set; }
        public MappingTargetDeck ConditionTwoTarget { get; set; }
        public byte[] ConditionTwoValue { get; set; }

        // set by Command
        private ValueUIType ledMinControllerRangeValueUIType { get { return ValueUIType; } }
        public byte[] LedMinControllerRange { get; set; }
        private ValueUIType ledMaxControllerRangeValueUIType { get { return ValueUIType; } }
        public byte[] LedMaxControllerRange { get; set; }

        // set by Control
        public int LedMinMidiRange { get; set; }
        public int LedMaxMidiRange { get; set; }

        /// <summary>
        /// Is optional.
        /// </summary>
        public bool LedInvert { get; set; }

        /// <summary>
        /// Is optional.
        /// </summary>
        public bool LedBlend { get; set; }

        /// <summary>
        /// Is optional. Determined dynamically to ensure integrity
        /// </summary>
        private ValueUIType unknownValueUIType { get { return (ControlType == MappingControlType.LED) ? ledMaxControllerRangeValueUIType : ValueUIType; } }
        
        /// <summary>
        /// Is optional.
        /// </summary>
        public MappingResolution Resolution { get; set; }

        /// <summary>
        /// Is optional.
        /// </summary>
        public bool UseFactoryMap { get; set; }


        public MappingSettings()
            : base("CMAD")
        {
            // values strongly depend on command, so it doesn't make sense to set default values here
            DeviceType = Enums.DeviceType.GenericMidi;
            Target = MappingTargetDeck.DeviceTarget;
            RotarySensitivity = 5f;
            ValueUIType = Enums.ValueUIType.ComboBox;
            SetValueTo = new byte[4];
            Comment = String.Empty;
            LedMinMidiRange = 0;
            LedMaxMidiRange = 127;
        }

        public MappingSettings(Stream stream)
            : base(stream)
        {            
            DeviceType = (DeviceType)stream.ReadInt32BigE();
            ControlType = (MappingControlType)stream.ReadInt32BigE();
            InteractionMode = (MappingInteractionMode)stream.ReadInt32BigE();
            Target = (MappingTargetDeck)stream.ReadInt32BigE();
            AutoRepeat = stream.ReadBoolBigE();
            Invert = stream.ReadBoolBigE();
            SoftTakeover = stream.ReadBoolBigE();
            RotarySensitivity = stream.ReadFloatBigE();
            RotaryAcceleration = stream.ReadFloatBigE();

            HasValueUI = stream.ReadBoolBigE();
            ValueUIType = (ValueUIType)stream.ReadInt32BigE();
            SetValueTo = stream.ReadBytesBigE(4);

            Comment = stream.ReadWideStringBigE();

            ConditionOneId = stream.ReadInt32BigE();
            ConditionOneTarget = (MappingTargetDeck)stream.ReadInt32BigE();
            ConditionOneValue = stream.ReadBytesBigE(4);
            ConditionTwoId = stream.ReadInt32BigE();
            ConditionTwoTarget = (MappingTargetDeck)stream.ReadInt32BigE();
            ConditionTwoValue = stream.ReadBytesBigE(4);

            var ledMinControllerRangeValueUIType = (ValueUIType)stream.ReadInt32BigE();
            LedMinControllerRange = stream.ReadBytesBigE(4);
            var ledMaxControllerRangeValueUIType = (ValueUIType)stream.ReadInt32BigE();
            LedMaxControllerRange = stream.ReadBytesBigE(4);

            LedMinMidiRange = stream.ReadInt32BigE();
            LedMaxMidiRange = stream.ReadInt32BigE();

            if (DeviceType != DeviceType.Proprietary_Synth)
            {
                LedInvert = stream.ReadBoolBigE();
                LedBlend = stream.ReadBoolBigE();

                if (DeviceType != DeviceType.Proprietary_Audio)
                {
                    // determined dynamically, see above
                    var unknownValueUIType = (ValueUIType)stream.ReadInt32BigE();

                    Resolution = (MappingResolution)stream.ReadInt32BigE();
                }

                if (DeviceType == DeviceType.GenericMidi)
                    UseFactoryMap = stream.ReadBoolBigE();
            }
        }


        public override void Write(Writer writer)
        {
            writer.BeginFrame(FrameId);

            writer.WriteBigE((int)DeviceType);
            writer.WriteBigE((int)ControlType);
            writer.WriteBigE((int)InteractionMode);
            writer.WriteBigE((int)Target);
            writer.WriteBigE(AutoRepeat);
            writer.WriteBigE(Invert);
            writer.WriteBigE(SoftTakeover);
            writer.WriteBigE(RotarySensitivity);
            writer.WriteBigE(RotaryAcceleration);
            writer.WriteBigE(HasValueUI);
            writer.WriteBigE((int)ValueUIType);
            writer.WriteBigE(SetValueTo);
            writer.WriteWideStringBigE(Comment);

            writer.WriteBigE(ConditionOneId);
            writer.WriteBigE((int)ConditionOneTarget);
            writer.WriteBigE(ConditionOneValue ?? new byte[4]);

            writer.WriteBigE(ConditionTwoId);
            writer.WriteBigE((int)ConditionTwoTarget);
            writer.WriteBigE(ConditionTwoValue ?? new byte[4]);
            
            writer.WriteBigE((int)((ledMinControllerRangeValueUIType == 0) ? ValueUIType : ledMinControllerRangeValueUIType));
            writer.WriteBigE(LedMinControllerRange ?? new byte[4]);
            writer.WriteBigE((int)((ledMaxControllerRangeValueUIType == 0) ? ValueUIType : ledMaxControllerRangeValueUIType));
            writer.WriteBigE(LedMaxControllerRange ?? new byte[4]);

            writer.WriteBigE(LedMinMidiRange);
            writer.WriteBigE(LedMaxMidiRange);

            if (DeviceType != DeviceType.Proprietary_Synth)
            {
                writer.WriteBigE(LedInvert);
                writer.WriteBigE(LedBlend);

                if (DeviceType != DeviceType.Proprietary_Audio)
                {
                    writer.WriteBigE((int)unknownValueUIType);
                    writer.WriteBigE((int)((Resolution == 0) ? MappingResolution.Default : Resolution));
                }

                if (DeviceType == DeviceType.GenericMidi)
                    writer.WriteBigE(UseFactoryMap);
            }

            writer.EndFrame();
        }
    }
}
