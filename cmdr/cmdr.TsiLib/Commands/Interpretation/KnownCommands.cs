﻿using System;
using System.Linq;
using cmdr.TsiLib.Ranges;
using cmdr.TsiLib.Enums;

namespace cmdr.TsiLib.Commands.Interpretation
{
    internal static class CommandExtensions
    {
        private static CommandDescriptionAttribute[] _attributes = new CommandDescriptionAttribute[1];
        private static Type _commandEnumType = typeof(KnownCommands);
        private static Type _commandAttributeType = typeof(CommandDescriptionAttribute);

        internal static CommandDescription GetCommandDescription(this KnownCommands cmd)
        {
            var field = _commandEnumType.GetField(cmd.ToString());
            if (field != null)
            {
                _attributes = (CommandDescriptionAttribute[])field.GetCustomAttributes(_commandAttributeType, false);
                if (_attributes.Any())
                {
                    var description = _attributes.First().Description;
                    description.Id = (int)cmd;
                    return description;
                }
            }

            // return Unknown Command
            return new CommandDescription
            {
                Id = (int)cmd,
                Category = (Categories)(-1),
                Name = "Unknown Command " + cmd,
                TargetType = (TargetType)(-1),
                InCommandType = typeof(UnknownInCommand),
                OutCommandType = typeof(UnknownOutCommand)
            };
        }
    }

    public enum KnownCommands
    {
        [CommandDescription(Categories.Mixer_XFader, "Position (X-Fader)", TargetType.Global, typeof(FloatInCommand<FloatRangeCentered>), typeof(FloatOutCommand<FloatRangeCentered>))]
        Mixer_XFader_PositionXFader = 5,

        [CommandDescription(Categories.Mixer, "Master Volume Adjust", TargetType.Global, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        Mixer_MasterVolumeAdjust = 6,

        [CommandDescription(Categories.Mixer, "Limiter On", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Mixer_LimiterOn = 7,

        [CommandDescription(Categories.Mixer, "Monitor Volume Adjust", TargetType.Global, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        Mixer_MonitorVolumeAdjust = 8,

        [CommandDescription(Categories.Layout, "Deck Focus Selector", TargetType.Global, typeof(EnumInCommand<Deck>), typeof(EnumOutCommand<Deck>))]
        Layout_DeckFocusSelector = 9,

        [CommandDescription(Categories.Mixer_XFader, "Curve Adjust", TargetType.Global, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        Mixer_XFader_CurveAdjust = 14,

        [CommandDescription(Categories.Mixer, "Monitor Mix Adjust", TargetType.Global, typeof(FloatInCommand<FloatRangeCentered>), typeof(FloatOutCommand<FloatRangeCentered>))]
        Mixer_MonitorMixAdjust = 17,

        [CommandDescription(Categories.DeckCommon, "Tempo Range Selector", TargetType.Track,typeof(EnumInCommand<TempoRange>), typeof(EnumOutCommand<TempoRange>))]
        DeckCommon_TempoRangeSelector = 19,

        [CommandDescription(Categories.AudioRecorder, "Gain Adjust (Audio Recorder)", TargetType.Global, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeCentered>))]
        AudioRecorder_GainAdjust = 29,

        [CommandDescription(Categories.MasterClock, "Auto Master Mode", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        MasterClock_AutoMasterMode = 60,

        [CommandDescription(Categories.MasterClock, "Clock Int/Ext", TargetType.Global, typeof(EnumInCommand<IntExt>), typeof(EnumOutCommand<IntExt>))]
        MasterClock_ClockIntExt = 62,

        [CommandDescription(Categories.MasterClock, "Set Master Tempo", TargetType.Global, typeof(FloatInCommand<FloatRangeCentered>), typeof(FloatOutCommand<FloatRangeCentered>))]
        MasterClock_SetMasterTempo = 64,

        [CommandDescription(Categories.MasterClock, "Master Tempo Selector", TargetType.Global, typeof(EnumInCommand<TempoSource>), typeof(EnumOutCommand<TempoSource>))]
        MasterClock_MasterTempoSelector = 69,

        [CommandDescription(Categories.DeckCommon, "Play/Pause (Deck Common)", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_PlayPause = 100,

        [CommandDescription(Categories.Mixer, "Volume Adjust", TargetType.Track, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        Mixer_VolumeAdjust = 102,

        [CommandDescription(Categories.DeckCommon, "Seek Position (Deck Common)", TargetType.Track, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        DeckCommon_SeekPosition = 103,

        [CommandDescription(Categories.Mixer, "Gain Adjust (Mixer)", TargetType.Track, typeof(FloatInCommand<FloatRangeCentered>), typeof(FloatOutCommand<FloatRangeCentered>))]
        Mixer_GainAdjust = 117,

        [CommandDescription(Categories.Mixer, "Auto-Gain Adjust", TargetType.Track, typeof(FloatInCommand<FloatRangeCentered>), typeof(FloatOutCommand<FloatRangeCentered>))]
        Mixer_AutoGainAdjust = 118,

        [CommandDescription(Categories.Mixer, "Monitor Cue On", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Mixer_MonitorCueOn = 119,

        [CommandDescription(Categories.DeckCommon, "Jog Turn", TargetType.Track, typeof(FloatInCommand<FloatRangeCentered>), typeof(FloatOutCommand<FloatRangeCentered>))]
        DeckCommon_JogTurn = 120,

        [CommandDescription(Categories.DeckCommon, "Tempo Sync", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        DeckCommon_TempoSync = 122,

        [CommandDescription(Categories.DeckCommon, "Tempo Adjust", TargetType.Track, typeof(FloatInCommand<FloatRangeCentered>), typeof(FloatOutCommand<FloatRangeCentered>))]
        DeckCommon_TempoAdjust = 123,

        [CommandDescription(Categories.DeckCommon, "Phase Sync", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        DeckCommon_PhaseSync = 124,

        [CommandDescription(Categories.DeckCommon, "Sync On", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_SyncOn = 125,

        [CommandDescription(Categories.Mixer, "Balance Adjust", TargetType.Track, typeof(FloatInCommand<FloatRangeCentered>), typeof(FloatOutCommand<FloatRangeCentered>))]
        Mixer_BalanceAdjust = 127,

        [CommandDescription(Categories.DeckCommon_Loop, "Loop Active On", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_Loop_LoopActiveOn = 202,

        [CommandDescription(Categories.DeckCommon, "Is In Active Loop", TargetType.Track, null, typeof(EnumOutCommand<OnOff>))]
        DeckCommon_IsInActiveLoop = 203,

        [CommandDescription(Categories.DeckCommon, "CUP (Cue Play)", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_CupCuePlay = 204,

        [CommandDescription(Categories.DeckCommon, "Cue", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_Cue = 206,

        [CommandDescription(Categories.TrackDeck_Cue, "Jump to active Cue Point (quantized)", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        TrackDeck_Cue_JumpToActiveCuePointQuantized = 209,

        [CommandDescription(Categories.PreviewPlayer, "Play/Pause (Preview Player)", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        PreviewPlayer_PlayPause = 210,

        [CommandDescription(Categories.PreviewPlayer, "Seek Position (Preview Player)", TargetType.Global, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        PreviewPlayer_SeekPosition = 211,

        [CommandDescription(Categories.TrackDeck_Cue, "Set Cue And Store As Next Hotcue", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        TrackDeck_Cue_SetCueAndStoreAsNextHotcue = 213,

        [CommandDescription(Categories.RemixDeck, "Quantize Selector", TargetType.Remix, typeof(EnumInCommand<QuantizeSize>), typeof(EnumOutCommand<QuantizeSize>))]
        RemixDeck_QuantizeSelector = 229,

        [CommandDescription(Categories.RemixDeck, "Quantize On (Remix Deck)", TargetType.Remix, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        RemixDeck_QuantizeOn = 230,

        [CommandDescription(Categories.DeckCommon_Submix, "Slot FX Amount", TargetType.Slot, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        DeckCommon_Submix_SlotFXAmount = 232,

        [CommandDescription(Categories.RemixDeck, "Load Set From List", TargetType.Remix, typeof(TriggerInCommand), typeof(TriggerOutCommand))] //TODO: Check if TargetType is Remix or Track
        RemixDeck_LoadSetFromList = 233,

        [CommandDescription(Categories.RemixDeck, "Save Remix Set", TargetType.Remix, typeof(TriggerInCommand), typeof(TriggerOutCommand))] //TODO: Check if TargetType is Remix or Track
        RemixDeck_SaveRemixSet = 234,

        [CommandDescription(Categories.RemixDeck, "Slot Punch On", TargetType.Slot, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        RemixDeck_SlotPunchOn = 235,

        [CommandDescription(Categories.RemixDeck, "Slot Stop/Delete/Load From List", TargetType.Slot, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        RemixDeck_SlotStopDeleteLoadFromList = 236,

        [CommandDescription(Categories.RemixDeck, "Slot Keylock On", TargetType.Slot, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        RemixDeck_SlotKeylockOn = 237,

        [CommandDescription(Categories.RemixDeck, "Slot Monitor On", TargetType.Slot, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        RemixDeck_SlotMonitorOn = 238,

        [CommandDescription(Categories.DeckCommon_Submix, "Slot FX On", TargetType.Slot, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_Submix_SlotFXOn = 239,

        [CommandDescription(Categories.RemixDeck_Legacy, "Play Mode All Slots", TargetType.Remix, typeof(EnumInCommand<PlayMode>), typeof(EnumOutCommand<PlayMode>))]
        RemixDeck_Legacy_PlayModeAllSlots = 242,

        [CommandDescription(Categories.RemixDeck_Legacy, "Slot Load From List", TargetType.Slot, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        RemixDeck_Legacy_SlotLoadFromList = 244,

        [CommandDescription(Categories.RemixDeck_Legacy, "Slot Capture From Deck", TargetType.Slot, typeof(EnumInCommand<Deck>), typeof(EnumOutCommand<Deck>))]
        RemixDeck_Legacy_SlotCaptureFromDeck = 245,

        [CommandDescription(Categories.RemixDeck_Legacy, "Slot Unload", TargetType.Slot, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        RemixDeck_Legacy_SlotUnload = 246,

        [CommandDescription(Categories.RemixDeck, "Slot State", TargetType.Slot, null, typeof(EnumOutCommand<SlotState>))]
        RemixDeck_SlotState = 247,

        [CommandDescription(Categories.DeckCommon_Submix, "Slot Filter Adjust", TargetType.Slot, typeof(FloatInCommand<FloatRangeCentered>), typeof(FloatOutCommand<FloatRangeCentered>))]
        RemixDeck_SlotFilterAdjust = 249,

        [CommandDescription(Categories.DeckCommon_Submix, "Slot Filter On", TargetType.Slot, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        RemixDeck_SlotFilterOn = 250,

        [CommandDescription(Categories.DeckCommon_Submix, "Slot Volume Adjust", TargetType.Slot, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        RemixDeck_SlotVolumeAdjust = 251,

        [CommandDescription(Categories.RemixDeck_Legacy, "Play All Slots", TargetType.Remix, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        RemixDeck_Legacy_PlayAllSlots = 255,

        [CommandDescription(Categories.RemixDeck_Legacy, "Trigger All Slots", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_Legacy_TriggerAllSlots = 256,

        [CommandDescription(Categories.RemixDeck_Legacy, "Slot Retrigger Play", TargetType.Slot, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        RemixDeck_Legacy_SlotRetriggerPlay = 258,

        [CommandDescription(Categories.DeckCommon_Submix, "Slot Mute On", TargetType.Slot, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        RemixDeck_SlotMuteOn = 259,

        [CommandDescription(Categories.RemixDeck, "Slot Retrigger", TargetType.Slot, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        RemixDeck_SlotRetrigger = 260,

        [CommandDescription(Categories.DeckCommon_Submix_Meters, "Slot Pre-Fader Level (L)", TargetType.Slot, null, typeof(FloatOutCommand<FloatRangeRelative>))]
        DeckCommon_Submix_Meters_SlotPreFaderLevelL = 261,

        [CommandDescription(Categories.DeckCommon_Submix_Meters, "Slot Pre-Fader Level (R)", TargetType.Slot, null, typeof(FloatOutCommand<FloatRangeRelative>))]
        DeckCommon_Submix_Meters_SlotPreFaderLevelR = 262,

        [CommandDescription(Categories.RemixDeck_Legacy, "Slot Capture From Loop Recorder", TargetType.Slot, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        RemixDeck_Legacy_SlotCaptureFromLoopRecorder = 263,

        [CommandDescription(Categories.RemixDeck_Legacy, "Slot Copy From Slot", TargetType.Slot, typeof(EnumInCommand<SlotCell>), typeof(EnumOutCommand<SlotCell>))]
        RemixDeck_Legacy_SlotCopyFromSlot = 264,

        [CommandDescription(Categories.RemixDeck, "Slot Play Mode", TargetType.Slot, typeof(EnumInCommand<PlayMode>), typeof(EnumOutCommand<PlayMode>))]
        RemixDeck_SlotPlayMode = 265,

        [CommandDescription(Categories.RemixDeck_Legacy, "Slot Size x2", TargetType.Slot, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        RemixDeck_Legacy_SlotSizex2 = 266,

        [CommandDescription(Categories.RemixDeck_Legacy, "Slot Size /2", TargetType.Slot, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        RemixDeck_Legacy_SlotSizeDiv2 = 267,

        [CommandDescription(Categories.RemixDeck_Legacy, "Slot Size Reset", TargetType.Slot, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        RemixDeck_Legacy_SlotSizeReset = 268,

        [CommandDescription(Categories.LoopRecorder, "Record (Loop Recorder)", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        LoopRecorder_Record = 280,

        [CommandDescription(Categories.LoopRecorder, "Size (Loop Recorder)", TargetType.Global, typeof(EnumInCommand<LoopRecorderSize>), typeof(EnumOutCommand<LoopRecorderSize>))]
        LoopRecorder_Size = 281,

        [CommandDescription(Categories.LoopRecorder, "Dry/Wet Adjust (Loop Recorder)", TargetType.Global, typeof(FloatInCommand<FloatRangeCentered>), typeof(FloatOutCommand<FloatRangeCentered>))]
        LoopRecorder_DryWetAdjust = 282,

        [CommandDescription(Categories.LoopRecorder, "Play/Pause (Loop Recorder)", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        LoopRecorder_PlayPause = 283,

        [CommandDescription(Categories.LoopRecorder, "Delete (Loop Recorder)", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        LoopRecorder_Delete = 284,

        [CommandDescription(Categories.LoopRecorder, "Undo/Redo (Loop Recorder)", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        LoopRecorder_UndoRedo = 287,

        [CommandDescription(Categories.Mixer, "Microphone Gain Adjust", TargetType.Global, typeof(FloatInCommand<FloatRangeCentered>), typeof(FloatOutCommand<FloatRangeCentered>))]
        Mixer_MicrophoneGainAdjust = 295,

        [CommandDescription(Categories.Mixer_EQ, "Low Adjust", TargetType.Track, typeof(FloatInCommand<FloatRangeCentered>), typeof(FloatOutCommand<FloatRangeCentered>))]
        Mixer_EQ_LowAdjust = 301,

        [CommandDescription(Categories.Mixer_EQ, "Mid Adjust", TargetType.Track, typeof(FloatInCommand<FloatRangeCentered>), typeof(FloatOutCommand<FloatRangeCentered>))]
        Mixer_EQ_MidAdjust = 302,

        [CommandDescription(Categories.Mixer_EQ, "High Adjust", TargetType.Track, typeof(FloatInCommand<FloatRangeCentered>), typeof(FloatOutCommand<FloatRangeCentered>))]
        Mixer_EQ_HighAdjust = 303,

        [CommandDescription(Categories.Mixer_EQ, "Low Kill", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Mixer_EQ_LowKill = 304,

        [CommandDescription(Categories.Mixer_EQ, "Mid Kill", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Mixer_EQ_MidKill = 305,

        [CommandDescription(Categories.Mixer_EQ, "High Kill", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Mixer_EQ_HighKill = 306,

        [CommandDescription(Categories.Mixer_EQ, "Mid Low Kill", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Mixer_EQ_MidLowKill = 307,

        [CommandDescription(Categories.Mixer_EQ, "Mid Low Adjust", TargetType.Track, typeof(FloatInCommand<FloatRangeCentered>), typeof(FloatOutCommand<FloatRangeCentered>))]
        Mixer_EQ_MidLowAdjust = 316,

        [CommandDescription(Categories.Mixer, "Filter On", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Mixer_FilterOn = 319,

        [CommandDescription(Categories.Mixer, "Filter Adjust", TargetType.Track, typeof(FloatInCommand<FloatRangeCentered>), typeof(FloatOutCommand<FloatRangeCentered>))]
        Mixer_FilterAdjust = 320,

        [CommandDescription(Categories.Mixer, "FX Unit 1 On", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Mixer_FXUnit1On = 321,

        [CommandDescription(Categories.Mixer, "FX Unit 2 On", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Mixer_FXUnit2On = 322,

        [CommandDescription(Categories.FXUnit, "Effect LFO Reset", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        FXUnit_EffectLFOReset = 323,

        [CommandDescription(Categories.FXUnit, "Routing Selector", TargetType.FX, typeof(EnumInCommand<FXRouting>), typeof(EnumOutCommand<FXRouting>))]
        FXUnit_RoutingSelector = 325,

        [CommandDescription(Categories.FXUnit, "FX Store Preset", TargetType.FX, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        FXUnit_FXStorePreset = 326,

        [CommandDescription(Categories.Mixer, "FX Unit 3 On", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Mixer_FXUnit3On = 338,

        [CommandDescription(Categories.Mixer, "FX Unit 4 On", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Mixer_FXUnit4On = 339,

        [CommandDescription(Categories.Mixer, "Deck Effect On", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Mixer_DeckEffectOn = 348,

        [CommandDescription(Categories.DeckCommon_Submix_Meters, "Slot Pre-Fader Level (L+R)", TargetType.Slot, null, typeof(FloatOutCommand<FloatRangeRelative>))]
        DeckCommon_Submix_Meters_SlotPreFaderLevelLR = 361,

        [CommandDescription(Categories.FXUnit, "Effect 1 Selector", TargetType.FX, typeof(EffectSelectorInCommand), typeof(EffectSelectorOutCommand))] // TODO: Needs special treatment, see EffectList.All
        FXUnit_Effect1Selector = 362,

        [CommandDescription(Categories.FXUnit, "Effect 2 Selector", TargetType.FX, typeof(EffectSelectorInCommand), typeof(EffectSelectorOutCommand))] // TODO: Needs special treatment, see EffectList.All
        FXUnit_Effect2Selector = 363,

        [CommandDescription(Categories.FXUnit, "Effect 3 Selector", TargetType.FX, typeof(EffectSelectorInCommand), typeof(EffectSelectorOutCommand))] // TODO: Needs special treatment, see EffectList.All
        FXUnit_Effect3Selector = 364,

        [CommandDescription(Categories.FXUnit, "Dry/Wet Adjust (FX Unit)", TargetType.FX, typeof(FloatInCommand<FloatRangeCentered>), typeof(FloatOutCommand<FloatRangeCentered>))] 
        FXUnit_DryWetAdjust = 365,

        [CommandDescription(Categories.FXUnit, "Knob 1", TargetType.FX, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        FXUnit_Knob1 = 366,

        [CommandDescription(Categories.FXUnit, "Knob 2", TargetType.FX, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        FXUnit_Knob2 = 367,

        [CommandDescription(Categories.FXUnit, "Knob 3", TargetType.FX, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        FXUnit_Knob3 = 368,

        [CommandDescription(Categories.FXUnit, "Unit On (FX Unit)", TargetType.FX, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        FXUnit_UnitOn = 369,
        
        [CommandDescription(Categories.FXUnit, "Button 1", TargetType.FX, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        FXUnit_Button1 = 370,

        [CommandDescription(Categories.FXUnit, "Button 2", TargetType.FX, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        FXUnit_Button2 = 371,

        [CommandDescription(Categories.FXUnit, "Button 3", TargetType.FX, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        FXUnit_Button3 = 372,

        [CommandDescription(Categories.TrackDeck, "Keylock On", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        TrackDeck_KeylockOn = 400,

        [CommandDescription(Categories.TrackDeck, "Key Adjust", TargetType.Track, typeof(FloatInCommand<FloatRangeCentered>), typeof(FloatOutCommand<FloatRangeCentered>))]
        TrackDeck_KeyAdjust = 402,

        [CommandDescription(Categories.DeckCommon, "Tempo Bend (stepless)", TargetType.Track, typeof(FloatInCommand<FloatRangeCentered>), typeof(FloatOutCommand<FloatRangeCentered>))]
        DeckCommon_TempoBendStepless = 404,

        [CommandDescription(Categories.TrackDeck, "Keylock On (Preserve Pitch)", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        TrackDeck_KeylockOnPreservePitch = 405,

        [CommandDescription(Categories.DeckCommon, "Tempo Bend", TargetType.Track, typeof(HoldEnumInCommand<UpDown>), typeof(EnumOutCommand<UpDown>))]
        DeckCommon_TempoBend = 406,

        [CommandDescription(Categories.DeckCommon, "Phase", TargetType.Track, null, typeof(FloatOutCommand<FloatRangeCenteredHalf>))]
        DeckCommon_Phase = 512,

        [CommandDescription(Categories.DeckCommon, "Beat Phase", TargetType.Track, null, typeof(FloatOutCommand<FloatRangeCenteredHalf>))]
        DeckCommon_BeatPhase = 513,

        [CommandDescription(Categories.TrackDeck, "Track End Warning", TargetType.Track, null, typeof(EnumOutCommand<OnOff>))]
        TrackDeck_TrackEndWarning = 520,

        #region Slot Cell Trigger Commands (601 - 664)

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 1 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell1Trigger = 601,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 2 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell2Trigger = 602,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 3 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell3Trigger = 603,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 4 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell4Trigger = 604,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 5 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell5Trigger = 605,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 6 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell6Trigger = 606,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 7 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell7Trigger = 607,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 8 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell8Trigger = 608,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 9 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell9Trigger = 609,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 10 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell10Trigger = 610,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 11 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell11Trigger = 611,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 12 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell12Trigger = 612,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 13 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell13Trigger = 613,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 14 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell14Trigger = 614,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 15 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell15Trigger = 615,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 16 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell16Trigger = 616,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 1 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell1Trigger = 617,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 2 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell2Trigger = 618,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 3 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell3Trigger = 619,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 4 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell4Trigger = 620,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 5 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell5Trigger = 621,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 6 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell6Trigger = 622,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 7 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell7Trigger = 623,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 8 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell8Trigger = 624,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 9 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell9Trigger = 625,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 10 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell10Trigger = 626,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 11 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell11Trigger = 627,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 12 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell12Trigger = 628,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 13 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell13Trigger = 629,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 14 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell14Trigger = 630,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 15 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell15Trigger = 631,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 16 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell16Trigger = 632,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 1 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell1Trigger = 633,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 2 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell2Trigger = 634,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 3 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell3Trigger = 635,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 4 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell4Trigger = 636,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 5 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell5Trigger = 637,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 6 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell6Trigger = 638,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 7 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell7Trigger = 639,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 8 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell8Trigger = 640,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 9 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell9Trigger = 641,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 10 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell10Trigger = 642,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 11 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell11Trigger = 643,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 12 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell12Trigger = 644,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 13 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell13Trigger = 645,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 14 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell14Trigger = 646,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 15 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell15Trigger = 647,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 16 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell16Trigger = 648,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 1 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell1Trigger = 649,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 2 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell2Trigger = 650,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 3 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell3Trigger = 651,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 4 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell4Trigger = 652,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 5 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell5Trigger = 653,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 6 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell6Trigger = 654,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 7 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell7Trigger = 655,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 8 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell8Trigger = 656,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 9 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell9Trigger = 657,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 10 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell10Trigger = 658,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 11 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell11Trigger = 659,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 12 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell12Trigger = 660,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 13 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell13Trigger = 661,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 14 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell14Trigger = 662,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 15 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell15Trigger = 663,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 16 Trigger", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell16Trigger = 664,

        #endregion

        #region Slot Cell State Commands (665 - 728)

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 1 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell1State = 665,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 2 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell2State = 666,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 3 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell3State = 667,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 4 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell4State = 668,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 5 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell5State = 669,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 6 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell6State = 670,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 7 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell7State = 671,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 8 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell8State = 672,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 9 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell9State = 673,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 10 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell10State = 674,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 11 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell11State = 675,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 12 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell12State = 676,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 13 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell13State = 677,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 14 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell14State = 678,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 15 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell15State = 679,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 16 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell16State = 680,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 1 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell1State = 681,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 2 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell2State = 682,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 3 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell3State = 683,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 4 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell4State = 684,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 5 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell5State = 685,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 6 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell6State = 686,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 7 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell7State = 687,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 8 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell8State = 688,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 9 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell9State = 689,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 10 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell10State = 690,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 11 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell11State = 691,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 12 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell12State = 692,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 13 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell13State = 693,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 14 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell14State = 694,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 15 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell15State = 695,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 16 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell16State = 696,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 1 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell1State = 697,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 2 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell2State = 698,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 3 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell3State = 699,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 4 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell4State = 700,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 5 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell5State = 701,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 6 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell6State = 702,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 7 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell7State = 703,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 8 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell8State = 704,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 9 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell9State = 705,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 10 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell10State = 706,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 11 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell11State = 707,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 12 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell12State = 708,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 13 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell13State = 709,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 14 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell14State = 710,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 15 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell15State = 711,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 16 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell16State = 712,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 1 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell1State = 713,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 2 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell2State = 714,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 3 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell3State = 715,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 4 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell4State = 716,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 5 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell5State = 717,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 6 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell6State = 718,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 7 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell7State = 719,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 8 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell8State = 720,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 9 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell9State = 721,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 10 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell10State = 722,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 11 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell11State = 723,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 12 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell12State = 724,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 13 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell13State = 725,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 14 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell14State = 726,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 15 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell15State = 727,

        [CommandDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 16 State", TargetType.Remix, null, typeof(EnumOutCommand<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell16State = 728,

        #endregion

        [CommandDescription(Categories.RemixDeck_DirectMapping, "Cell Load Modifier", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_CellLoadModifier = 729,

        [CommandDescription(Categories.RemixDeck_DirectMapping, "Cell Delete Modifier", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_CellDeleteModifier = 730,

        [CommandDescription(Categories.RemixDeck_DirectMapping, "Cell Reverse Modifier", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_CellReverseModifier = 731,

        [CommandDescription(Categories.RemixDeck_DirectMapping, "Cell Capture Modifier", TargetType.Remix, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        RemixDeck_DirectMapping_CellCaptureModifier = 732,

        [CommandDescription(Categories.RemixDeck, "Sample Page Selector", TargetType.Remix, typeof(EnumInCommand<SamplePage>), typeof(EnumOutCommand<SamplePage>))]
        RemixDeck_SamplePageSelector = 733,

        [CommandDescription(Categories.DeckCommon_FreezeMode, "Freeze Slice Count Adjust", TargetType.Track, typeof(EnumInCommand<FreezeSliceCount>), typeof(EnumOutCommand<FreezeSliceCount>))]
        DeckCommon_FreezeMode_FreezeSliceCountAdjust = 802,

        [CommandDescription(Categories.DeckCommon_FreezeMode, "Freeze Mode On", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_FreezeMode_FreezeModeOn = 803,

        [CommandDescription(Categories.DeckCommon_FreezeMode, "Freeze Slice Size Adjust", TargetType.Track, typeof(EnumInCommand<FreezeSliceSize>), typeof(EnumOutCommand<FreezeSliceSize>))]
        DeckCommon_FreezeMode_FreezeSliceSizeAdjust = 804,

        [CommandDescription(Categories.DeckCommon_FreezeMode, "Slice Trigger 1 (Freeze Mode)", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_FreezeMode_SliceTrigger1 = 810,

        [CommandDescription(Categories.DeckCommon_FreezeMode, "Slice Trigger 2 (Freeze Mode)", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_FreezeMode_SliceTrigger2 = 811,

        [CommandDescription(Categories.DeckCommon_FreezeMode, "Slice Trigger 3 (Freeze Mode)", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_FreezeMode_SliceTrigger3 = 812,

        [CommandDescription(Categories.DeckCommon_FreezeMode, "Slice Trigger 4 (Freeze Mode)", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_FreezeMode_SliceTrigger4 = 813,

        [CommandDescription(Categories.DeckCommon_FreezeMode, "Slice Trigger 5 (Freeze Mode)", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_FreezeMode_SliceTrigger5 = 814,

        [CommandDescription(Categories.DeckCommon_FreezeMode, "Slice Trigger 6 (Freeze Mode)", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_FreezeMode_SliceTrigger6 = 815,

        [CommandDescription(Categories.DeckCommon_FreezeMode, "Slice Trigger 7 (Freeze Mode)", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_FreezeMode_SliceTrigger7 = 816,

        [CommandDescription(Categories.DeckCommon_FreezeMode, "Slice Trigger 8 (Freeze Mode)", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_FreezeMode_SliceTrigger8 = 817,

        [CommandDescription(Categories.DeckCommon_FreezeMode, "Slice Trigger 9 (Freeze Mode)", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_FreezeMode_SliceTrigger9 = 818,

        [CommandDescription(Categories.DeckCommon_FreezeMode, "Slice Trigger 10 (Freeze Mode)", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_FreezeMode_SliceTrigger10 = 819,

        [CommandDescription(Categories.DeckCommon_FreezeMode, "Slice Trigger 11 (Freeze Mode)", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_FreezeMode_SliceTrigger11 = 820,

        [CommandDescription(Categories.DeckCommon_FreezeMode, "Slice Trigger 12 (Freeze Mode)", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_FreezeMode_SliceTrigger12 = 821,

        [CommandDescription(Categories.DeckCommon_FreezeMode, "Slice Trigger 13 (Freeze Mode)", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_FreezeMode_SliceTrigger13 = 822,

        [CommandDescription(Categories.DeckCommon_FreezeMode, "Slice Trigger 14 (Freeze Mode)", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_FreezeMode_SliceTrigger14 = 823,

        [CommandDescription(Categories.DeckCommon_FreezeMode, "Slice Trigger 15 (Freeze Mode)", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_FreezeMode_SliceTrigger15 = 824,

        [CommandDescription(Categories.DeckCommon_FreezeMode, "Slice Trigger 16 (Freeze Mode)", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_FreezeMode_SliceTrigger16 = 825,

        [CommandDescription(Categories.Global_MidiControls_Buttons, "MIDI Button 1", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Global_MidiControls_Buttons_MidiButton1 = 850,

        [CommandDescription(Categories.Global_MidiControls_Buttons, "MIDI Button 2", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Global_MidiControls_Buttons_MidiButton2 = 851,

        [CommandDescription(Categories.Global_MidiControls_Buttons, "MIDI Button 3", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Global_MidiControls_Buttons_MidiButton3 = 852,

        [CommandDescription(Categories.Global_MidiControls_Buttons, "MIDI Button 4", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Global_MidiControls_Buttons_MidiButton4 = 853,

        [CommandDescription(Categories.Global_MidiControls_Buttons, "MIDI Button 5", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Global_MidiControls_Buttons_MidiButton5 = 854,

        [CommandDescription(Categories.Global_MidiControls_Buttons, "MIDI Button 6", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Global_MidiControls_Buttons_MidiButton6 = 855,

        [CommandDescription(Categories.Global_MidiControls_Buttons, "MIDI Button 7", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Global_MidiControls_Buttons_MidiButton7 = 856,

        [CommandDescription(Categories.Global_MidiControls_Buttons, "MIDI Button 8", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Global_MidiControls_Buttons_MidiButton8 = 857,

        [CommandDescription(Categories.Global_MidiControls_Knobs, "MIDI Knob 1", TargetType.Global, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        Global_MidiControls_Knobs_MidiKnob1 = 858,

        [CommandDescription(Categories.Global_MidiControls_Knobs, "MIDI Knob 2", TargetType.Global, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        Global_MidiControls_Knobs_MidiKnob2 = 859,

        [CommandDescription(Categories.Global_MidiControls_Knobs, "MIDI Knob 3", TargetType.Global, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        Global_MidiControls_Knobs_MidiKnob3 = 860,

        [CommandDescription(Categories.Global_MidiControls_Knobs, "MIDI Knob 4", TargetType.Global, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        Global_MidiControls_Knobs_MidiKnob4 = 861,

        [CommandDescription(Categories.Global_MidiControls_Knobs, "MIDI Knob 5", TargetType.Global, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        Global_MidiControls_Knobs_MidiKnob5 = 862,

        [CommandDescription(Categories.Global_MidiControls_Knobs, "MIDI Knob 6", TargetType.Global, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        Global_MidiControls_Knobs_MidiKnob6 = 863,

        [CommandDescription(Categories.Global_MidiControls_Knobs, "MIDI Knob 7", TargetType.Global, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        Global_MidiControls_Knobs_MidiKnob7 = 864,

        [CommandDescription(Categories.Global_MidiControls_Knobs, "MIDI Knob 8", TargetType.Global, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        Global_MidiControls_Knobs_MidiKnob8 = 865,

        [CommandDescription(Categories.Global_MidiControls_Faders, "MIDI Fader 1", TargetType.Global, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        Global_MidiControls_Knobs_MidiFader1 = 866,

        [CommandDescription(Categories.Global_MidiControls_Faders, "MIDI Fader 2", TargetType.Global, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        Global_MidiControls_Knobs_MidiFader2 = 867,

        [CommandDescription(Categories.Global_MidiControls_Faders, "MIDI Fader 3", TargetType.Global, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        Global_MidiControls_Knobs_MidiFader3 = 868,

        [CommandDescription(Categories.Global_MidiControls_Faders, "MIDI Fader 4", TargetType.Global, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        Global_MidiControls_Knobs_MidiFader4 = 869,

        [CommandDescription(Categories.Global_MidiControls_Faders, "MIDI Fader 5", TargetType.Global, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        Global_MidiControls_Knobs_MidiFader5 = 870,

        [CommandDescription(Categories.Global_MidiControls_Faders, "MIDI Fader 6", TargetType.Global, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        Global_MidiControls_Knobs_MidiFader6 = 871,

        [CommandDescription(Categories.Global_MidiControls_Faders, "MIDI Fader 7", TargetType.Global, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        Global_MidiControls_Knobs_MidiFader7 = 872,

        [CommandDescription(Categories.Global_MidiControls_Faders, "MIDI Fader 8", TargetType.Global, typeof(FloatInCommand<FloatRangeRelative>), typeof(FloatOutCommand<FloatRangeRelative>))]
        Global_MidiControls_Knobs_MidiFader8 = 873,

        [CommandDescription(Categories.RemixDeck_Legacy, "Slot Size Adjust", TargetType.Slot, typeof(EnumInCommand<SlotSize>), typeof(EnumOutCommand<SlotSize>))]
        RemixDeck_Legacy_SlotSizeAdjust = 2000,

        [CommandDescription(Categories.RemixDeck, "Capture Source Selector", TargetType.Remix, typeof(EnumInCommand<CaptureSource>), typeof(EnumOutCommand<CaptureSource>))]
        RemixDeck_CaptureSourceSelector = 2002,

        [CommandDescription(Categories.RemixDeck, "Slot Trigger Type", TargetType.Slot, typeof(EnumInCommand<SlotTriggerType>), typeof(EnumOutCommand<SlotTriggerType>))]
        RemixDeck_SlotTriggerType = 2003,

        [CommandDescription(Categories.RemixDeck, "Slot Capture/Trigger/Mute", TargetType.Slot, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        RemixDeck_SlotCaptureTriggerMute = 2004,

        [CommandDescription(Categories.AudioRecorder, "Cut (Audio Recorder)", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        AudioRecorder_Cut = 2055,

        [CommandDescription(Categories.AudioRecorder, "Record/Stop (Audio Recorder)", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        AudioRecorder_RecordStop = 2056,

        [CommandDescription(Categories.Global, "Broadcasting On", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Global_BroadcastingOn = 2057,

        [CommandDescription(Categories.Mixer_XFader, "Auto X-Fade Left", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Mixer_XFader_AutoXFadeLeft = 2113,

        [CommandDescription(Categories.Mixer_XFader, "Auto X-Fade Right", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Mixer_XFader_AutoXFadeRight = 2114,

        [CommandDescription(Categories.DeckCommon, "Load Next", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        DeckCommon_LoadNext = 2176,

        [CommandDescription(Categories.DeckCommon, "Load Previous", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        DeckCommon_LoadPrevious = 2177,

        [CommandDescription(Categories.DeckCommon, "Unload (Deck Common)", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        DeckCommon_Unload = 2178,

        [CommandDescription(Categories.PreviewPlayer, "Unload (Preview Player)", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        PreviewPlayer_Unload = 2179,

        [CommandDescription(Categories.DeckCommon, "Jog Touch On", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_JogTouchOn = 2187,

        [CommandDescription(Categories.DeckCommon_Loop, "Loop Set", TargetType.Track, typeof(HoldInCommand), typeof(TriggerOutCommand))]
        DeckCommon_Loop_LoopSet = 2192,

        [CommandDescription(Categories.DeckCommon_Loop, "Loop Size Selector", TargetType.Track, typeof(EnumInCommand<LoopSize>), typeof(EnumOutCommand<LoopSize>))]
        DeckCommon_Loop_LoopSizeSelector = 2196,

        [CommandDescription(Categories.TrackDeck_Grid, "Autogrid", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        TrackDeck_Grid_Autogrid = 2237,

        [CommandDescription(Categories.TrackDeck_Grid, "BPM Adjust", TargetType.Track, typeof(FloatInCommand<FloatRangeCentered>), typeof(FloatOutCommand<FloatRangeCentered>))]
        TrackDeck_Grid_BPMAdjust = 2238,

        [CommandDescription(Categories.TrackDeck_Grid, "Beat Tap (Track Deck - Grid)", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        TrackDeck_Grid_BeatTap = 2240,

        [CommandDescription(Categories.TrackDeck_Grid, "BPM Lock On", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        TrackDeck_Grid_BPMLockOn = 2241,

        [CommandDescription(Categories.TrackDeck_Grid, "Set Grid Marker", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        TrackDeck_Grid_SetGridMarker = 2248,

        [CommandDescription(Categories.TrackDeck_Grid, "Delete Grid Marker", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        TrackDeck_Grid_DeleteGridMarker = 2249,

        [CommandDescription(Categories.TrackDeck_Grid, "Tick On (Track Deck - Grid)", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        TrackDeck_Grid_TickOn = 2252,

        [CommandDescription(Categories.TrackDeck_Grid, "Move Grid Marker", TargetType.Track, typeof(FloatInCommand<FloatRangeCentered>), typeof(FloatOutCommand<FloatRangeCentered>))]
        TrackDeck_Grid_MoveGridMarker = 2253,

        [CommandDescription(Categories.TrackDeck_Grid, "Reset BPM", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        TrackDeck_Grid_ResetBPM = 2254,

        [CommandDescription(Categories.TrackDeck_Grid, "Copy Phase From Tempo Master", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        TrackDeck_Grid_CopyPhaseFromTempoMaster = 2255,

        [CommandDescription(Categories.TrackDeck_Grid, "BPM x2", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        TrackDeck_Grid_BPMx2 = 2258,

        [CommandDescription(Categories.TrackDeck_Grid, "BPM /2", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        TrackDeck_Grid_BPMDiv2 = 2259,

        [CommandDescription(Categories.DeckCommon_Timecode, "Scratch Control On", TargetType.Track, typeof(EnumInCommand<ScratchControl>), typeof(EnumOutCommand<ScratchControl>))]
        DeckCommon_Timecode_ScratchControlOn = 2288,

        [CommandDescription(Categories.DeckCommon, "Set As Tempo Master", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        DeckCommon_SetAsTempoMaster = 2293,

        [CommandDescription(Categories.DeckCommon, "Advanced Panel Tab Selector", TargetType.Track, typeof(EnumInCommand<AdvancedPanelTab>), typeof(EnumOutCommand<AdvancedPanelTab>))]
        DeckCommon_AdvancedPanelTabSelector = 2298,

        [CommandDescription(Categories.DeckCommon, "Advanced Panel Toggle", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_AdvancedPanelToggle = 2299,

        [CommandDescription(Categories.DeckCommon, "Deck Size Selector", TargetType.Track, typeof(EnumInCommand<DeckSize>), typeof(EnumOutCommand<DeckSize>))]
        DeckCommon_DeckSizeSelector = 2300,

        [CommandDescription(Categories.FXUnit, "FX Unit Mode Selector", TargetType.FX, typeof(EnumInCommand<FXUnitMode>), typeof(EnumOutCommand<FXUnitMode>))]
        FXUnit_FXUnitModeSelector = 2301,

        [CommandDescription(Categories.DeckCommon, "Deck Flavor Selector", TargetType.Track, typeof(EnumInCommand<DeckFlavor>), typeof(EnumOutCommand<DeckFlavor>))]
        DeckCommon_DeckFlavorSelector = 2302,

        [CommandDescription(Categories.DeckCommon_Timecode, "Platter/Scope View Selector", TargetType.Track, typeof(EnumInCommand<PlatterScopeView>), typeof(EnumOutCommand<PlatterScopeView>))]
        DeckCommon_Timecode_PlatterScopeViewSelector = 2305,

        [CommandDescription(Categories.TrackDeck_Cue, "Jump To Next/Prev Cue/Loop", TargetType.Track, typeof(EnumInCommand<JumpDirection>), typeof(EnumOutCommand<JumpDirection>))]
        TrackDeck_Cue_JumpToNextPrevCueLoop = 2306,

        [CommandDescription(Categories.TrackDeck_Cue, "Store Floating Cue/Loop As Next Hotcue", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        TrackDeck_Cue_StoreFloatingCueLoopAsNextHotcue = 2308,

        [CommandDescription(Categories.TrackDeck_Cue, "Delete Current Hotcue", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        TrackDeck_Cue_DeleteCurrentHotcue = 2309,

        [CommandDescription(Categories.Global, "Snap On", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Global_SnapOn = 2311,

        [CommandDescription(Categories.Global, "Quantize On (Global)", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Global_QuantizeOn = 2313,

        [CommandDescription(Categories.TrackDeck_Cue, "Map Hotcue", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        TrackDeck_Cue_MapHotcue = 2315,

        [CommandDescription(Categories.DeckCommon_Loop, "Loop Size Select + Set", TargetType.Track, typeof(EnumInCommand<LoopSize>), typeof(EnumOutCommand<LoopSize>))]
        DeckCommon_Loop_LoopSizeSelectSet = 2317,

        [CommandDescription(Categories.DeckCommon_Loop, "Backward Loop Size Select + Set", TargetType.Track, typeof(EnumInCommand<LoopSize>), typeof(EnumOutCommand<LoopSize>))]
        DeckCommon_Loop_BackwardLoopSizeSelectSet = 2318,

        [CommandDescription(Categories.TrackDeck_Cue, "Cue Type Selector", TargetType.Track, typeof(EnumInCommand<CueType>), typeof(EnumOutCommand<CueType>))]
        TrackDeck_Cue_CueTypeSelector = 2327,

        [CommandDescription(Categories.TrackDeck_Cue, "Select/Set+Store Hotcue", TargetType.Track, typeof(HoldEnumInCommand<Hotcue>), typeof(EnumOutCommand<Hotcue>))]
        TrackDeck_Cue_SelectSetStoreHotcue = 2328,

        [CommandDescription(Categories.TrackDeck_Cue, "Delete Hotcue", TargetType.Track, typeof(EnumInCommand<Hotcue>), typeof(EnumOutCommand<Hotcue>))]
        TrackDeck_Cue_DeleteHotcue = 2331,

        [CommandDescription(Categories.TrackDeck_Cue, "Hotcue 1 Type", TargetType.Track, null, typeof(EnumOutCommand<HotcueType>))]
        TrackDeck_Cue_Hotcue1Type = 2333,

        [CommandDescription(Categories.TrackDeck_Cue, "Hotcue 2 Type", TargetType.Track, null, typeof(EnumOutCommand<HotcueType>))]
        TrackDeck_Cue_Hotcue2Type = 2334,

        [CommandDescription(Categories.TrackDeck_Cue, "Hotcue 3 Type", TargetType.Track, null, typeof(EnumOutCommand<HotcueType>))]
        TrackDeck_Cue_Hotcue3Type = 2335,

        [CommandDescription(Categories.TrackDeck_Cue, "Hotcue 4 Type", TargetType.Track, null, typeof(EnumOutCommand<HotcueType>))]
        TrackDeck_Cue_Hotcue4Type = 2336,

        [CommandDescription(Categories.TrackDeck_Cue, "Hotcue 5 Type", TargetType.Track, null, typeof(EnumOutCommand<HotcueType>))]
        TrackDeck_Cue_Hotcue5Type = 2337,

        [CommandDescription(Categories.TrackDeck_Cue, "Hotcue 6 Type", TargetType.Track, null, typeof(EnumOutCommand<HotcueType>))]
        TrackDeck_Cue_Hotcue6Type = 2338,

        [CommandDescription(Categories.TrackDeck_Cue, "Hotcue 7 Type", TargetType.Track, null, typeof(EnumOutCommand<HotcueType>))]
        TrackDeck_Cue_Hotcue7Type = 2339,

        [CommandDescription(Categories.TrackDeck_Cue, "Hotcue 8 Type", TargetType.Track, null, typeof(EnumOutCommand<HotcueType>))]
        TrackDeck_Cue_Hotcue8Type = 2340,

        [CommandDescription(Categories.DeckCommon, "Flux Mode On", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        DeckCommon_FluxModeOn = 2350,

        [CommandDescription(Categories.DeckCommon_Move, "Move", TargetType.Track, typeof(EnumInCommand<MoveDirection>), typeof(EnumOutCommand<MoveDirection>))]
        DeckCommon_Move_Move = 2351,

        [CommandDescription(Categories.DeckCommon_Move, "Size Selector (Cue/Loop)", TargetType.Track, typeof(EnumInCommand<CuePointOrLoopMoveSize>), typeof(EnumOutCommand<CuePointOrLoopMoveSize>))]
        DeckCommon_Move_SizeSelectorCueLoop = 2372,

        [CommandDescription(Categories.DeckCommon_Move, "Beatjump", TargetType.Track, typeof(HoldEnumInCommand<MoveSize>), typeof(EnumOutCommand<MoveSize>))]
        DeckCommon_Move_Beatjump = 2380,

        [CommandDescription(Categories.DeckCommon_Move, "Mode Selector (Deck Common Move)", TargetType.Track, typeof(EnumInCommand<MoveMode>), typeof(EnumOutCommand<MoveMode>))]
        DeckCommon_Move_ModeSelector = 2391,

        [CommandDescription(Categories.DeckCommon_Loop, "Loop In/Set Cue", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        DeckCommon_Loop_LoopInSetCue = 2392,

        [CommandDescription(Categories.DeckCommon_Loop, "Loop Out", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        DeckCommon_Loop_LoopOut = 2393,

        [CommandDescription(Categories.TrackDeck, "Load", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        TrackDeck_Load = 2395,

        [CommandDescription(Categories.TrackDeck, "Duplicate Track Deck A", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        TrackDeck_DuplicateTrackDeckA = 2401,

        [CommandDescription(Categories.TrackDeck, "Duplicate Track Deck B", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        TrackDeck_DuplicateTrackDeckB = 2402,

        [CommandDescription(Categories.TrackDeck, "Duplicate Track Deck C", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        TrackDeck_DuplicateTrackDeckC = 2403,

        [CommandDescription(Categories.TrackDeck, "Duplicate Track Deck D", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        TrackDeck_DuplicateTrackDeckD = 2404,

        [CommandDescription(Categories.Mixer_XFader, "Assign Left", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Mixer_XFader_AssignLeft = 2408,

        [CommandDescription(Categories.Mixer_XFader, "Assign Right", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Mixer_XFader_AssignRight = 2409,

        [CommandDescription(Categories.MasterClock, "Clock Send", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        MasterClock_ClockSend = 2468,

        [CommandDescription(Categories.MasterClock, "Beat Tap (Master Clock)", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        MasterClock_BeatTap = 2469,

        [CommandDescription(Categories.MasterClock, "Tick On (Master Clock)", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        MasterClock_TickOn = 2470,

        [CommandDescription(Categories.MasterClock, "Clock Trigger MIDI Sync", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        MasterClock_ClockTriggerMIDISync = 2473,

        [CommandDescription(Categories.MasterClock, "Tempo Bend Up", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        MasterClock_TempoBendUp = 2476,

        [CommandDescription(Categories.MasterClock, "Tempo Bend Down", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        MasterClock_TempoBendDown = 2477,

        [CommandDescription(Categories.Modifier, "Modifier #1", TargetType.Global, typeof(EnumInCommand<ModifierValue>), typeof(EnumOutCommand<ModifierValue>))]
        Modifier_Modifier1 = 2548,

        [CommandDescription(Categories.Modifier, "Modifier #2", TargetType.Global, typeof(EnumInCommand<ModifierValue>), typeof(EnumOutCommand<ModifierValue>))]
        Modifier_Modifier2 = 2549,

        [CommandDescription(Categories.Modifier, "Modifier #3", TargetType.Global, typeof(EnumInCommand<ModifierValue>), typeof(EnumOutCommand<ModifierValue>))]
        Modifier_Modifier3 = 2550,

        [CommandDescription(Categories.Modifier, "Modifier #4", TargetType.Global, typeof(EnumInCommand<ModifierValue>), typeof(EnumOutCommand<ModifierValue>))]
        Modifier_Modifier4 = 2551,

        [CommandDescription(Categories.Modifier, "Modifier #5", TargetType.Global, typeof(EnumInCommand<ModifierValue>), typeof(EnumOutCommand<ModifierValue>))]
        Modifier_Modifier5 = 2552,

        [CommandDescription(Categories.Modifier, "Modifier #6", TargetType.Global, typeof(EnumInCommand<ModifierValue>), typeof(EnumOutCommand<ModifierValue>))]
        Modifier_Modifier6 = 2553,

        [CommandDescription(Categories.Modifier, "Modifier #7", TargetType.Global, typeof(EnumInCommand<ModifierValue>), typeof(EnumOutCommand<ModifierValue>))]
        Modifier_Modifier7 = 2554,

        [CommandDescription(Categories.Modifier, "Modifier #8", TargetType.Global, typeof(EnumInCommand<ModifierValue>), typeof(EnumOutCommand<ModifierValue>))]
        Modifier_Modifier8 = 2555,

        [CommandDescription(Categories.Layout, "Toggle Last Focus", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Layout_ToggleLastFocus = 2588,

        [CommandDescription(Categories.DeckCommon, "Deck Is Loaded", TargetType.Track, null, typeof(EnumOutCommand<OnOff>))]
        DeckCommon_DeckIsLoaded = 2591,

        [CommandDescription(Categories.Mixer_Meters, "Deck Pre-Fader Level (L)", TargetType.Track, null, typeof(FloatOutCommand<FloatRangeRelative>))]
        Mixer_Meters_DeckPreFaderLevelL = 2688,

        [CommandDescription(Categories.Mixer_Meters, "Deck Pre-Fader Level (R)", TargetType.Track, null, typeof(FloatOutCommand<FloatRangeRelative>))]
        Mixer_Meters_DeckPreFaderLevelR = 2689,

        [CommandDescription(Categories.Mixer_Meters, "Deck Post-Fader Level (L)", TargetType.Track, null, typeof(FloatOutCommand<FloatRangeRelative>))]
        Mixer_Meters_DeckPostFaderLevelL = 2690,

        [CommandDescription(Categories.Mixer_Meters, "Deck Post-Fader Level (R)", TargetType.Track, null, typeof(FloatOutCommand<FloatRangeRelative>))]
        Mixer_Meters_DeckPostFaderLevelR = 2691,

        [CommandDescription(Categories.Mixer_Meters, "Mixer Level (L)", TargetType.Global, null, typeof(FloatOutCommand<FloatRangeRelative>))]
        Mixer_Meters_MixerLevelL = 2692,

        [CommandDescription(Categories.Mixer_Meters, "Mixer Level (R)", TargetType.Global, null, typeof(FloatOutCommand<FloatRangeRelative>))]
        Mixer_Meters_MixerLevelR = 2693,

        [CommandDescription(Categories.Mixer_Meters, "Master Out Level (L)", TargetType.Global, null, typeof(FloatOutCommand<FloatRangeRelative>))]
        Mixer_Meters_MasterOutLevelL = 2694,

        [CommandDescription(Categories.Mixer_Meters, "Master Out Level (R)", TargetType.Global, null, typeof(FloatOutCommand<FloatRangeRelative>))]
        Mixer_Meters_MasterOutLevelR = 2695,

        [CommandDescription(Categories.Mixer_Meters, "Master Out Clip (L)", TargetType.Global, null, typeof(FloatOutCommand<FloatRangeRelative>))]
        Mixer_Meters_MasterOutClipL = 2696,

        [CommandDescription(Categories.Mixer_Meters, "Master Out Clip (R)", TargetType.Global, null, typeof(FloatOutCommand<FloatRangeRelative>))]
        Mixer_Meters_MasterOutClipR = 2697,

        [CommandDescription(Categories.Mixer_Meters, "Record Input Level (L)", TargetType.Global, null, typeof(FloatOutCommand<FloatRangeRelative>))]
        Mixer_Meters_RecordInputLevelL = 2698,

        [CommandDescription(Categories.Mixer_Meters, "Record Input Level (R)", TargetType.Global, null, typeof(FloatOutCommand<FloatRangeRelative>))]
        Mixer_Meters_RecordInputLevelR = 2699,

        [CommandDescription(Categories.Mixer_Meters, "Record Input Clip (L)", TargetType.Global, null, typeof(FloatOutCommand<FloatRangeRelative>))]
        Mixer_Meters_RecordInputClipL = 2700,

        [CommandDescription(Categories.Mixer_Meters, "Record Input Clip (R)", TargetType.Global, null, typeof(FloatOutCommand<FloatRangeRelative>))]
        Mixer_Meters_RecordInputClipR = 2701,

        [CommandDescription(Categories.Mixer_Meters, "Master Out Level (L+R)", TargetType.Global, null, typeof(FloatOutCommand<FloatRangeRelative>))]
        Mixer_Meters_MasterOutLevelLR = 2703,

        [CommandDescription(Categories.Mixer_Meters, "Master Out Clip (L+R)", TargetType.Global, null, typeof(FloatOutCommand<FloatRangeRelative>))]
        Mixer_Meters_MasterOutClipLR = 2704,

        [CommandDescription(Categories.Mixer_Meters, "Deck Pre-Fader Level (L+R)", TargetType.Track, null, typeof(FloatOutCommand<FloatRangeRelative>))]
        Mixer_Meters_DeckPreFaderLevelLR = 2712,

        [CommandDescription(Categories.Mixer_Meters, "Deck Post-Fader Level (L+R)", TargetType.Track, null, typeof(FloatOutCommand<FloatRangeRelative>))]
        Mixer_Meters_DeckPostFaderLevelLR = 2713,

        [CommandDescription(Categories.Global, "Show Slider Values On", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Global_ShowSliderValuesOn = 2748,

        [CommandDescription(Categories.DeckCommon, "Analyze Loaded Track", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        DeckCommon_AnalyzeLoadedTrack = 2798,

        [CommandDescription(Categories.Mixer, "Auto-Gain View On", TargetType.Track, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Mixer_AutoGainViewOn = 2807,

        [CommandDescription(Categories.Global, "Send Monitor State", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Global_SendMonitorState = 3048,

        [CommandDescription(Categories.Global, "Save Traktor Settings", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Global_SaveTraktorSettings = 3072,

        [CommandDescription(Categories.DeckCommon, "Load Selected (Deck Timecode)", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        DeckCommon_LoadSelected = 3076,

        [CommandDescription(Categories.Browser_Tree, "Check Consistency", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_Tree_CheckConsistency = 3077,

        [CommandDescription(Categories.AudioRecorder, "Load Last Recording", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        AudioRecorder_LoadLastRecording = 3084,

        [CommandDescription(Categories.PreviewPlayer, "Load Selected (Preview Player)", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        PreviewPlayer_LoadSelected = 3137,

        [CommandDescription(Categories.PreviewPlayer, "Load Preview Player into Deck", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        PreviewPlayer_LoadPreviewPlayerIntoDeck = 3139,

        [CommandDescription(Categories.Browser_List, "Consolidate", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_Consolidate = 3172,

        [CommandDescription(Categories.Browser_List, "Select Up/Down (Browser List)", TargetType.Global, typeof(EnumInCommand<ListNavigation>), typeof(EnumOutCommand<ListNavigation>))]
        Browser_List_SelectUpDown = 3200,

        [CommandDescription(Categories.Browser_List, "Select Page Up/Down (Browser List)", TargetType.Global, typeof(EnumInCommand<UpDown>), typeof(EnumOutCommand<UpDown>))]
        Browser_List_SelectPageUpDown = 3201,

        [CommandDescription(Categories.Browser_List, "Select Top/Bottom (Browser List)", TargetType.Global, typeof(EnumInCommand<TopBottom>), typeof(EnumOutCommand<TopBottom>))]
        Browser_List_SelectTopBottom = 3202,

        [CommandDescription(Categories.Browser_List, "Select Extend Up/Down (Browser List)", TargetType.Global, typeof(EnumInCommand<ListOffset>), typeof(EnumOutCommand<ListOffset>))]
        Browser_List_SelectExtendUpDown = 3203,

        [CommandDescription(Categories.Browser_List, "Select Extend Page Up/Down (Browser List)", TargetType.Global, typeof(EnumInCommand<UpDown>), typeof(EnumOutCommand<UpDown>))]
        Browser_List_SelectExtendPageUpDown = 3204,

        [CommandDescription(Categories.Browser_List, "Select Extend Top/Bottom (Browser List)", TargetType.Global, typeof(EnumInCommand<TopBottom>), typeof(EnumOutCommand<TopBottom>))]
        Browser_List_SelectExtendTopBottom = 3205,

        [CommandDescription(Categories.Browser_List, "Select All", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_SelectAll = 3206,

        [CommandDescription(Categories.Browser_List, "Delete (Browser List)", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_Delete = 3211,

        [CommandDescription(Categories.Browser_List, "Reset Played-State (Browser List)", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_ResetPlayedState = 3212,

        [CommandDescription(Categories.Browser_List, "Analyze (Browser List)", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_Analyze = 3213,

        [CommandDescription(Categories.Browser_Tree, "Save Collection", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_Tree_SaveCollection = 3214,

        [CommandDescription(Categories.Browser_List, "Edit (Browser List)", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_Edit = 3215,

        [CommandDescription(Categories.Browser_List, "Relocate (Browser List)", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_Relocate = 3216,

        [CommandDescription(Categories.Browser_List, "Add As Track To Collection", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_AddAsTrackToCollection = 3217,

        [CommandDescription(Categories.Browser_List, "Search", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_Search = 3221,

        [CommandDescription(Categories.Browser_List, "Search Clear", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_SearchClear = 3222,

        [CommandDescription(Categories.Browser_List, "Expand Remix Set", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_ExpandRemixSet = 3223,

        [CommandDescription(Categories.Browser_List, "Clear", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_Clear = 3224,

        [CommandDescription(Categories.Browser_List, "Analysis Lock (Browser List)", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_AnalysisLock = 3231,

        [CommandDescription(Categories.Browser_List, "Analysis Unlock (Browser List)", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_AnalysisUnlock = 3232,

        [CommandDescription(Categories.Browser_Tree, "Refresh Explorer Folder Content", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_Tree_RefreshExplorerFolderContent = 3233,

        [CommandDescription(Categories.Browser_Tree, "Select Up/Down (Browser Tree)", TargetType.Global, typeof(EnumInCommand<TreeNavigation>), typeof(EnumOutCommand<TreeNavigation>))]
        Browser_Tree_SelectUpDown = 3328,

        [CommandDescription(Categories.Browser_Tree, "Select Expand/Collapse", TargetType.Global, typeof(EnumInCommand<ExpandCollapse>), typeof(EnumOutCommand<ExpandCollapse>))]
        Browser_Tree_SelectExpandCollapse = 3329,

        [CommandDescription(Categories.Browser_Tree, "Delete (Browser Tree)", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_Tree_Delete = 3336,

        [CommandDescription(Categories.Browser_Tree, "Reset Played-State (Browser Tree)", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_Tree_ResetPlayedState = 3337,

        [CommandDescription(Categories.Browser_Tree, "Analyze (Browser Tree)", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_Tree_Analyze = 3338,

        [CommandDescription(Categories.Browser_Tree, "Edit (Browser Tree)", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_Tree_Edit = 3339,

        [CommandDescription(Categories.Browser_Tree, "Relocate (Browser Tree)", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_Tree_Relocate = 3340,

        [CommandDescription(Categories.Browser_Tree, "Import Music Folders", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_Tree_ImportMusicFolders = 3345,

        [CommandDescription(Categories.Browser_Tree, "Export", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_Tree_Export = 3346,

        [CommandDescription(Categories.Browser_Tree, "Export Printable", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_Tree_ExportPrintable = 3348,

        [CommandDescription(Categories.Browser_Tree, "Rename Playlist Or Folder", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_Tree_RenamePlaylistOrFolder = 3349,

        [CommandDescription(Categories.Browser_Tree, "Import Collection", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_Tree_ImportCollection = 3353,

        [CommandDescription(Categories.Browser_List, "Search In Playlists", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_SearchInPlaylists = 3357,

        [CommandDescription(Categories.Browser_List, "Show In Explorer", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_ShowInExplorer = 3358,

        [CommandDescription(Categories.Browser_List, "Jump To Current Track", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_JumpToCurrentTrack = 3366,

        [CommandDescription(Categories.Browser_Tree, "Restore AutoGain", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_Tree_RestoreAutoGain = 3367,

        [CommandDescription(Categories.Browser_Tree, "Create Playlist", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_Tree_CreatePlaylist = 3373,

        [CommandDescription(Categories.Browser_Tree, "Delete Playlist", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_Tree_DeletePlaylist = 3374,

        [CommandDescription(Categories.Browser_Tree, "Create Playlist Folder", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_Tree_CreatePlaylistFolder = 3375,

        [CommandDescription(Categories.Browser_Tree, "Delete Playlist Folder", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_Tree_DeletePlaylistFolder = 3376,

        [CommandDescription(Categories.Browser_Favorites, "Selector (Browser Favorites)", TargetType.Global, typeof(EnumInCommand<Favorite>), typeof(EnumOutCommand<Favorite>))]
        Browser_Favorites_Selector = 3456,

        [CommandDescription(Categories.Browser_Favorites, "Add Folder To Favorites", TargetType.Global, typeof(EnumInCommand<Favorite>), typeof(EnumOutCommand<Favorite>))]
        Browser_Favorites_AddFolderToFavorites = 3457,

        [CommandDescription(Categories.Browser_Tree, "Add Folder To Music Folders", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_Tree_AddFolderToMusicFolders = 3458,

        [CommandDescription(Categories.Browser_List, "Append To Preparation List", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_AppendToPreparationList = 3460,

        [CommandDescription(Categories.Browser_List, "Add As Next To Preparation List", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_AddAsNextToPreparationList = 3461,

        [CommandDescription(Categories.Browser_List, "Add As Loop To Collection", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_AddAsLoopToCollection = 3469,

        [CommandDescription(Categories.Browser_List, "Add As One-Shot Sample To Collection", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_AddAsOneShotSampleToCollection = 3470,

        [CommandDescription(Categories.Browser_List, "Set To Track", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_SetToTrack = 3472,

        [CommandDescription(Categories.Browser_List, "Set To Looped Sample", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_SetToLoopedSample = 3473,

        [CommandDescription(Categories.Browser_List, "Set To One-Shot Sample", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_SetToOneShotSample = 3474,

        [CommandDescription(Categories.Browser_List, "Export As Remix Set", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_List_ExportAsRemixSet = 3475,

        [CommandDescription(Categories.Browser_Tree, "Analysis Lock (Browser Tree)", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_Tree_AnalysisLock = 3477,

        [CommandDescription(Categories.Browser_Tree, "Analysis Unlock (Browser Tree)", TargetType.Global, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        Browser_Tree_AnalysisUnlock = 3478,

        [CommandDescription(Categories.TrackDeck, "Waveform Zoom Adjust", TargetType.Track, typeof(FloatInCommand<FloatRangeCentered>), typeof(FloatOutCommand<FloatRangeCentered>))]
        TrackDeck_WaveformZoomAdjust = 4162,

        [CommandDescription(Categories.Layout, "Layout Selector", TargetType.Global, typeof(EnumInCommand<Enums.Layout>), typeof(EnumOutCommand<Enums.Layout>))]
        Layout_LayoutSelector = 4208,

        [CommandDescription(Categories.Layout, "Only Browser On", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Layout_OnlyBrowserOn = 4209,

        [CommandDescription(Categories.Layout, "Fullscreen On", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Layout_FullscreenOn = 4210,

        [CommandDescription(Categories.Global, "Tool Tips On", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Global_ToolTipsOn = 4211,

        [CommandDescription(Categories.DeckCommon_Timecode, "Playback Mode Int/Rel/Abs", TargetType.Track, typeof(EnumInCommand<PlaybackMode>), typeof(EnumOutCommand<PlaybackMode>))]
        DeckCommon_Timecode_PlaybackModeIntRelAbs = 5129,

        [CommandDescription(Categories.DeckCommon_Timecode, "Calibrate", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        DeckCommon_Timecode_Calibrate = 5144,

        [CommandDescription(Categories.DeckCommon_Timecode, "Reset Tempo Offset", TargetType.Track, typeof(TriggerInCommand), typeof(TriggerOutCommand))]
        DeckCommon_Timecode_ResetTempoOffset = 5154,

        [CommandDescription(Categories.Global, "Cruise Mode On", TargetType.Global, typeof(OnOffInCommand), typeof(EnumOutCommand<OnOff>))]
        Global_CruiseModeOn = 8194
    }
}
