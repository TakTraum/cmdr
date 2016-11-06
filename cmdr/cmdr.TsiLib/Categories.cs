using System.ComponentModel;

namespace cmdr.TsiLib
{
    public enum Categories
    {
        Unknown, 

        [Description("Global")]
        Global,

        [Description("Global->MIDI Controls")]
        Global_MidiControls,

        [Description("Global->MIDI Controls->Buttons")]
        Global_MidiControls_Buttons,

        [Description("Global->MIDI Controls->Knobs")]
        Global_MidiControls_Knobs,

        [Description("Global->MIDI Controls->Faders")]
        Global_MidiControls_Faders,

        [Description("Mixer")]
        Mixer,

        [Description("Mixer->X-Fader")]
        Mixer_XFader,

        [Description("Mixer->EQ")]
        Mixer_EQ,

        [Description("Mixer->Meters")]
        Mixer_Meters,

        [Description("Layout")]
        Layout,

        [Description("Deck Common")]
        DeckCommon,

        [Description("Deck Common->Loop")]
        DeckCommon_Loop,

        [Description("Deck Common->Move")]
        DeckCommon_Move,

        [Description("Deck Common->Freeze Mode")]
        DeckCommon_FreezeMode,

        [Description("Deck Common->Timecode")]
        DeckCommon_Timecode,

        [Description("Deck Common->Submix")]
        DeckCommon_Submix,

        [Description("Deck Common->Submix->Meters")]
        DeckCommon_Submix_Meters,

        [Description("Audio Recorder")]
        AudioRecorder,

        [Description("Master Clock")]
        MasterClock,

        #region new in Traktor 2.11

        [Description("Master Clock->Ableton Link")]
        MasterClock_AbletonLink,

        #endregion

        [Description("Preview Player")]
        PreviewPlayer,

        [Description("Track Deck")]
        TrackDeck,

        [Description("Track Deck->Cue")]
        TrackDeck_Cue,

        [Description("Track Deck->Grid")]
        TrackDeck_Grid,

        [Description("Remix Deck")]
        RemixDeck,

        [Description("Remix Deck->Legacy")]
        RemixDeck_Legacy,

        [Description("Remix Deck->Direct Mapping")]
        RemixDeck_DirectMapping,

        [Description("Remix Deck->Direct Mapping->Slot1")]
        RemixDeck_DirectMapping_Slot1,

        [Description("Remix Deck->Direct Mapping->Slot2")]
        RemixDeck_DirectMapping_Slot2,

        [Description("Remix Deck->Direct Mapping->Slot3")]
        RemixDeck_DirectMapping_Slot3,

        [Description("Remix Deck->Direct Mapping->Slot4")]
        RemixDeck_DirectMapping_Slot4,

        #region new in Traktor 2.11

        [Description("Remix Deck->Step Sequencer")]
        RemixDeck_StepSequencer,

        #endregion

        [Description("Loop Recorder")]
        LoopRecorder,

        [Description("FX Unit")]
        FXUnit,

        [Description("Modifier")]
        Modifier,

        [Description("Browser")]
        Browser,

        [Description("Browser->Tree")]
        Browser_Tree,

        [Description("Browser->List")]
        Browser_List,

        [Description("Browser->Favorites")]
        Browser_Favorites
    }
}
