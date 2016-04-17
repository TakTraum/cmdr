using System.ComponentModel;

namespace cmdr.TsiLib.Commands
{
    public static class Effects
    {
        public enum All
        {
            Reverb,

            [Description("Reverb T3")]
            ReverbT3,
            
            Iceverb,
            
            Bouncer,

            [Description("Auto Bouncer")]
            AutoBouncer,
            
            Delay,

            [Description("Delay T3")]
            DelayT3,

            [Description("Tape Delay")]
            TapeDelay,

            [Description("Ramp Delay")]
            RampDelay,
            
            Echo,
            
            Phaser,

            [Description("Phaser Flux")]
            PhaserFlux,

            [Description("Phaser Pulse")]
            PhaserPulse,
            
            Flanger,

            [Description("Flanger Flux")]
            FlangerFlux,

            [Description("Flanger Pulse")]
            FlangerPulse,

            [Description("Beatmasher 2")]
            Beatmasher2,
            
            Filter,

            [Description("Filter LFO")]
            FilterLFO,

            [Description("Filter Pulse")]
            FilterPulse,
            
            [Description("Filter:92")]
            Filter92,

            [Description("Filter:92 LFO")]
            Filter92LFO,

            [Description("Filter:92 Pulse")]
            Filter92Pulse,
            
            Gater,
            
            [Description("Digital LoFi")]
            DigitalLoFi,
            
            [Description("Reverse Grain")]
            ReverseGrain,
            
            [Description("Turntable FX")]
            TurntableFX,
            
            Ringmodulator,
            
            [Description("Transpose Stretch")]
            TransposeStretch,
            
            [Description("Pitch-Shifting")]
            PitchShifting,

            [Description("Time-Stretching")]
            TimeStretching,
            
            [Description("Mulholland-Drive")]
            MulhollandDrive,
            
            [Description("Beat Slicer")]
            BeatSlicer,
            
            [Description("Formant Filter")]
            FormantFilter,
            
            [Description("Peak Filter")]
            PeakFilter,

            // Macro Effects

            [Description("Bass-o-matic")]
            BassOMatic,
            
            [Description("Dark Matter")]
            DarkMatter,
            
            [Description("Event Horizon")]
            EventHorizon,
              
            [Description("Flight Test")]
            FlightTest,
            
            [Description("Granu Phase")]
            GranuPhase,
            
            [Description("Laser Slicer")]
            LaserSlicer,
            
            [Description("Polar Wind")]
            PolarWind,
            
            [Description("Strretch Fast")]
            StrretchFast,
            
            [Description("Strrretch Slow")]
            StrrretchSlow,
            
            Wormhole,
            
            Zzzurp
        }
    }
}
