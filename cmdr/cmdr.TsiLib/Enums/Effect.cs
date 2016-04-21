using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdr.TsiLib.Enums
{
    public enum Effect
    {
        [Description("No Effect")]
        NoEffect = 0,

        Flanger = 1,

        [Description("Flanger Pulse")]
        FlangerPulse = 2,

        [Description("Flanger Flux")]
        FlangerFlux = 3,

        Phaser = 4,

        [Description("Phaser Pulse")]
        PhaserPulse = 5,

        [Description("Phaser Flux")]
        PhaserFlux = 6,

        [Description("Filter LFO")]
        FilterLFO = 7,

        [Description("Filter Pulse")]
        FilterPulse = 8,

        Filter = 9,

        [Description("Filter:92 LFO")]
        Filter92LFO = 10,

        [Description("Filter:92 Pulse")]
        Filter92Pulse = 11,

        [Description("Filter:92")]
        Filter92 = 12,

        Delay = 13,

        [Description("Beatmasher 2")]
        Beatmasher2 = 14,

        [Description("Reverse Grain")]
        ReverseGrain = 15,

        [Description("Turntable FX")]
        TurntableFX = 16,

        Gater = 17,

        Iceverb = 18,

        Reverb = 19,

        [Description("Digital LoFi")]
        DigitalLoFi = 24,

        Ringmodulator = 25,

        [Description("Mulholland-Drive")]
        MulhollandDrive = 26,

        [Description("Transpose Stretch")]
        TransposeStretch = 27,

        [Description("Reverb T3")]
        ReverbT3 = 28,

        [Description("Delay T3")]
        DelayT3 = 29,

        [Description("Beat Slicer")]
        BeatSlicer = 31,

        [Description("Formant Filter")]
        FormantFilter = 32,

        [Description("Peak Filter")]
        PeakFilter = 33,

        [Description("Tape Delay")]
        TapeDelay = 34,

        [Description("Ramp Delay")]
        RampDelay = 35,

        [Description("Auto Bouncer")]
        AutoBouncer = 36,

        Bouncer = 37,

        // Macro Effects

        Wormhole = 42,

        Zzzurp = 56,

        [Description("Laser Slicer")]
        LaserSlicer = 58,

        [Description("Granu Phase")]
        GranuPhase = 59,

        [Description("Strretch Fast")]
        StrretchFast = 62,

        [Description("Bass-o-matic")]
        BassOMatic = 74,

        [Description("Polar Wind")]
        PolarWind = 76,

        [Description("Strrretch Slow")]
        StrrretchSlow = 84,

        [Description("Dark Matter")]
        DarkMatter = 85,

        [Description("Event Horizon")]
        EventHorizon = 87,

        [Description("Flight Test")]
        FlightTest = 89,

        
        // old effects, ids unknown:
        // gaps:    20, 21, 22, 23, 30, 38, 39 , 40, 41
        /*
        Echo,

        [Description("Pitch-Shifting")]
        PitchShifting,

        [Description("Time-Stretching")]
        TimeStretching,
         */
    }
}
