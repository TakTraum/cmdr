namespace cmdr.MidiLib.Core.MidiIO.Definitions
{
    internal enum ControllerType
    {
        BankSelectMsb,// 0..127 
        ModulationWheelMsb,// 0..127
        BreathControlMsb,// 0..127
        ContinuousController,// 0..127
        FootControllerMsb,// 0..127
        PortamentoTimeMsb,// 0..127
        DataEntrySliderMsb,// 0..127
        MainVolumeMsb,// 0..127
        StereoBalanceMsb,// 0..127
        ContinuousControllerNo9,// 0..127
        PanMsb,// 0=left 64=center 127=right
        ExpressionMsb,// 0..127
        EffectControl1Msb,// 0..127
        EffectControl2Msb,// 0..127
        ContinuousControllerNo14,// 0..127
        ContinuousControllerNo15,// 0..127
        GeneralPurposeSlider1,// 0..127 
        GeneralPurposeSlider2,// 0..127 
        GeneralPurposeSlider3,// 0..127
        GeneralPurposeSlider4,// 0..127
        ContinuousControllerNo20,// 0..127
        ContinuousControllerNo21,// 0..127
        ContinuousControllerNo22,// 0..127
        ContinuousControllerNo23,// 0..127
        ContinuousControllerNo24,// 0..127
        ContinuousControllerNo25,// 0..127
        ContinuousControllerNo26,// 0..127
        ContinuousControllerNo27,// 0..127
        ContinuousControllerNo28,// 0..127
        ContinuousControllerNo29,// 0..127
        ContinuousControllerNo30,// 0..127
        ContinuousControllerNo31,// 0..127
        BankSelectLsb,// 0..127 usually ignored
        ModulationWheelLsb,// 0..127 
        BreathControlLsb,// 0..127
        ContinuousControllerNo3Lsb,// 0..127
        FootControllerLsb,// 0..127
        PortamentoTimeLsb,// 0..127
        DataEntrySliderLsb,// 0..127
        MainVolumeLsb,// 0..127 usually ignored
        StereoBalanceLsb,// 0..127
        ContinuousControllerNo9Lsb,// 0..127 
        PanLsb,// 0..127 usually ignored
        ExpressionLsb,// (sub-Volume) 0..127 usually ignored
        EffectControl1Lsb,// 0..127
        EffectControl2Lsb,// 0..127
        ContinuousControllerNo14Lsb,// 0..127 
        ContinuousControllerNo15Lsb,// 0..127 
        ContinuousControllerNo16,// 0..127 
        ContinuousControllerNo17,// 0..127
        ContinuousControllerNo18,// 0..127
        ContinuousControllerNo19,// 0..127
        ContinuousControllerNo20Lsb,// 0..127 
        ContinuousControllerNo21Lsb,// 0..127 
        ContinuousControllerNo22Lsb,// 0..127 
        ContinuousControllerNo23Lsb,// 0..127 
        ContinuousControllerNo24Lsb,// 0..127 
        ContinuousControllerNo25Lsb,// 0..127 
        ContinuousControllerNo26Lsb,// 0..127 
        ContinuousControllerNo27Lsb,// 0..127 
        ContinuousControllerNo28Lsb,// 0..127 
        ContinuousControllerNo29Lsb,// 0..127 
        ContinuousControllerNo30Lsb,// 0..127 
        ContinuousControllerNo31Lsb,// 0..127 
        HoldPedal,// (Sustain) on/off 0..63=off 64..127=on
        Portamento,// on/off 0..63=off 64..127=on
        Sustenuto,// Pedal on/off 0..63=off 64..127=on
        Soft,// Pedal on/off 0..63=off 64..127=on
        Legato,// Pedal on/off 0..63=off 64..127=on
        Hold,// Pedal 2 on/off 0..63=off 64..127=on
        SoundVariation,// 0..127
        SoundTimbre,// 0..127
        SoundReleaseTime,// 0..127
        SoundAttackTime,// 0..127
        SoundBrighness,// 0..127
        SoundControl6,// 0..127
        SoundControl7,// 0..127
        SoundControl8,// 0..127
        SoundControl9,// 0..127
        SoundControl10,// 0..127
        GeneralPurposeButton80,// 0..63=off 64..127=on
        GeneralPurposeButton81,// 0..63=off 64..127=on
        GeneralPurposeButton82,// 0..63=off 64..127=on
        GeneralPurposeButton83,// 0..63=off 64..127=on
        Undefined84,// on/off 0..63=off 64..127=on
        Undefined85,// on/off 0..63=off 64..127=on
        Undefined86,// on/off 0..63=off 64..127=on
        Undefined87,// on/off 0..63=off 64..127=on
        Undefined88,// on/off 0..63=off 64..127=on
        Undefined89,// on/off 0..63=off 64..127=on
        Undefined90,// on/off 0..63=off 64..127=on
        EffectsLevel,// 0..127
        TremuloLevel,// 0..127
        ChorusLevel,// 0..127
        CelesteLevel,// 0..127
        PhaserLevel,// 0..127
        DataEntryIncrement,// ignored
        DataEntryDecrement,// ignored
        NrpnMsb,//0..127
        NrpnLsb,// 0..127
        RpnMsb,// 0..127
        RpnLsb,// 0..127
        Undefined102,//
        Undefined103,//
        Undefined104,//
        Undefined105,//
        Undefined106,//
        Undefined107,//
        Undefined108,//
        Undefined109,//
        Undefined110,//
        Undefined111,//
        Undefined112,//
        Undefined113,//
        Undefined114,//
        Undefined115,//
        Undefined116,//
        Undefined117,//
        Undefined118,//
        Undefined119,//
        AllSoundOff,// ignored
        AllControllersOff,// ignored
        LocalControl,// 0..63=off 64..127=on
        AllNotesOff,// ignored
        OmniModeOff,// ignored
        OmniModeOn,// ignored
        MonophonicModeOn,// **
        PolyphonicModeOn// (mono=off) ignored
    }
}

