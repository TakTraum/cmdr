using System;
using System.Linq;
using cmdr.TsiLib.Enums;

namespace cmdr.TsiLib.Conditions.Interpretation
{
    internal static class ConditionExtensions
    {
        private static ConditionDescriptionAttribute[] _attributes = new ConditionDescriptionAttribute[1];
        private static Type _conditionEnumType = typeof(KnownConditions);
        private static Type _conditionAttributeType = typeof(ConditionDescriptionAttribute);

        internal static ConditionDescription GetConditionDescription(this KnownConditions cond)
        {
            var field = _conditionEnumType.GetField(cond.ToString());
            if (field != null)
            {
                _attributes = (ConditionDescriptionAttribute[])field.GetCustomAttributes(_conditionAttributeType, false);
                if (_attributes.Any())
                {
                    var description = _attributes.First().Description;
                    description.Id = (int)cond;
                    return description;
                }
            }

            // return Unknown Command
            return new ConditionDescription
            {
                Id = (int)cond,
                Category = (Categories)(-1),
                Name = "Unknown Condition " + cond,
                TargetType = (TargetType)(-1),
                ConditionType = typeof(IntCondition),
            };
        }
    }


    public enum KnownConditions
    {
        [ConditionDescription(Categories.DeckCommon, "Tempo Range", TargetType.Track, typeof(EnumCondition<TempoRange>))]
        DeckCommon_TempoRange = 19,

        [ConditionDescription(Categories.DeckCommon, "Play/Pause (Deck Common)", TargetType.Track, typeof(EnumCondition<OnOff>))]
        DeckCommon_PlayPause = 100,

        [ConditionDescription(Categories.DeckCommon, "Is In Active Loop", TargetType.Track, typeof(EnumCondition<OnOff>))]
        DeckCommon_IsInActiveLoop = 203,

        [ConditionDescription(Categories.RemixDeck, "Slot State", TargetType.Slot, typeof(EnumCondition<SlotState>))]
        RemixDeck_SlotState = 247,

        #region Slot Cell State Conditions (665 - 728)

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 1 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell1State = 665,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 2 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell2State = 666,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 3 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell3State = 667,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 4 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell4State = 668,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 5 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell5State = 669,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 6 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell6State = 670,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 7 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell7State = 671,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 8 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell8State = 672,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 9 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell9State = 673,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 10 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell10State = 674,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 11 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell11State = 675,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 12 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell12State = 676,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 13 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell13State = 677,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 14 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell14State = 678,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 15 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell15State = 679,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot1, "Slot 1 Cell 16 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot1_Slot1Cell16State = 680,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 1 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell1State = 681,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 2 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell2State = 682,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 3 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell3State = 683,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 4 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell4State = 684,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 5 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell5State = 685,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 6 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell6State = 686,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 7 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell7State = 687,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 8 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell8State = 688,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 9 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell9State = 689,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 10 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell10State = 690,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 11 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell11State = 691,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 12 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell12State = 692,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 13 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell13State = 693,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 14 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell14State = 694,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 15 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell15State = 695,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot2, "Slot 2 Cell 16 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot2_Slot2Cell16State = 696,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 1 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell1State = 697,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 2 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell2State = 698,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 3 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell3State = 699,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 4 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell4State = 700,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 5 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell5State = 701,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 6 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell6State = 702,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 7 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell7State = 703,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 8 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell8State = 704,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 9 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell9State = 705,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 10 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell10State = 706,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 11 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell11State = 707,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 12 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell12State = 708,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 13 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell13State = 709,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 14 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell14State = 710,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 15 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell15State = 711,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot3, "Slot 3 Cell 16 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot3_Slot3Cell16State = 712,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 1 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell1State = 713,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 2 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell2State = 714,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 3 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell3State = 715,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 4 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell4State = 716,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 5 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell5State = 717,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 6 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell6State = 718,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 7 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell7State = 719,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 8 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell8State = 720,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 9 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell9State = 721,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 10 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell10State = 722,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 11 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell11State = 723,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 12 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell12State = 724,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 13 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell13State = 725,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 14 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell14State = 726,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 15 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell15State = 727,

        [ConditionDescription(Categories.RemixDeck_DirectMapping_Slot4, "Slot 4 Cell 16 State", TargetType.Remix, typeof(EnumCondition<SlotCellState>))]
        RemixDeck_DirectMapping_Slot4_Slot4Cell16State = 728,

        #endregion

        [ConditionDescription(Categories.RemixDeck, "Sample Page Selector", TargetType.Remix, typeof(EnumCondition<SamplePage>))]
        RemixDeck_SamplePageSelector = 733,


        [ConditionDescription(Categories.DeckCommon, "Scratch Control On", TargetType.Track, typeof(EnumCondition<OnOff>))]
        DeckCommon_Timecode_ScratchControlOn = 2288,

        [ConditionDescription(Categories.FXUnit, "FX Unit Mode", Enums.TargetType.FX, typeof(EnumCondition<FXUnitMode>))]
        FXUnit_FXUnitMode = 2301,

        [ConditionDescription(Categories.DeckCommon, "Deck Flavor", TargetType.Track, typeof(EnumCondition<DeckFlavor>))]
        DeckCommon_DeckFlavor = 2302,

        [ConditionDescription(Categories.TrackDeck_Cue, "Hotcue 1 Type", TargetType.Track, typeof(EnumCondition<HotcueType>))]
        TrackDeck_Cue_Hotcue1Type = 2333,

        [ConditionDescription(Categories.TrackDeck_Cue, "Hotcue 2 Type", TargetType.Track, typeof(EnumCondition<HotcueType>))]
        TrackDeck_Cue_Hotcue2Type = 2334,

        [ConditionDescription(Categories.TrackDeck_Cue, "Hotcue 3 Type", TargetType.Track, typeof(EnumCondition<HotcueType>))]
        TrackDeck_Cue_Hotcue3Type = 2335,

        [ConditionDescription(Categories.TrackDeck_Cue, "Hotcue 4 Type", TargetType.Track, typeof(EnumCondition<HotcueType>))]
        TrackDeck_Cue_Hotcue4Type = 2336,

        [ConditionDescription(Categories.TrackDeck_Cue, "Hotcue 5 Type", TargetType.Track, typeof(EnumCondition<HotcueType>))]
        TrackDeck_Cue_Hotcue5Type = 2337,

        [ConditionDescription(Categories.TrackDeck_Cue, "Hotcue 6 Type", TargetType.Track, typeof(EnumCondition<HotcueType>))]
        TrackDeck_Cue_Hotcue6Type = 2338,

        [ConditionDescription(Categories.TrackDeck_Cue, "Hotcue 7 Type", TargetType.Track, typeof(EnumCondition<HotcueType>))]
        TrackDeck_Cue_Hotcue7Type = 2339,

        [ConditionDescription(Categories.TrackDeck_Cue, "Hotcue 8 Type", TargetType.Track, typeof(EnumCondition<HotcueType>))]
        TrackDeck_Cue_Hotcue8Type = 2340,

        [ConditionDescription(Categories.TrackDeck_Cue, "Cue/Loop Move Mode", TargetType.Track, typeof(EnumCondition<CueLoopMoveMode>))]
        TrackDeck_Cue_LoopMoveMode = 2391,

        [ConditionDescription(Categories.Modifier, "M1", Enums.TargetType.Global, typeof(EnumCondition<ModifierValue>))]
        Modifier_Modifier1 = 2548,

        [ConditionDescription(Categories.Modifier, "M2", Enums.TargetType.Global, typeof(EnumCondition<ModifierValue>))]
        Modifier_Modifier2 = 2549,

        [ConditionDescription(Categories.Modifier, "M3", Enums.TargetType.Global, typeof(EnumCondition<ModifierValue>))]
        Modifier_Modifier3 = 2550,

        [ConditionDescription(Categories.Modifier, "M4", Enums.TargetType.Global, typeof(EnumCondition<ModifierValue>))]
        Modifier_Modifier4 = 2551,

        [ConditionDescription(Categories.Modifier, "M5", Enums.TargetType.Global, typeof(EnumCondition<ModifierValue>))]
        Modifier_Modifier5 = 2552,

        [ConditionDescription(Categories.Modifier, "M6", Enums.TargetType.Global, typeof(EnumCondition<ModifierValue>))]
        Modifier_Modifier6 = 2553,

        [ConditionDescription(Categories.Modifier, "M7", Enums.TargetType.Global, typeof(EnumCondition<ModifierValue>))]
        Modifier_Modifier7 = 2554,

        [ConditionDescription(Categories.Modifier, "M8", Enums.TargetType.Global, typeof(EnumCondition<ModifierValue>))]
        Modifier_Modifier8 = 2555,

    }
}
