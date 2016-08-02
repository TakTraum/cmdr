using System;
using System.Collections.Generic;
using System.Linq;
using cmdr.TsiLib.Commands.Interpretation;
using cmdr.TsiLib.Controls;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Format;

namespace cmdr.TsiLib.Commands
{
    public abstract class ACommand
    {
        internal Format.MappingSettings RawSettings;

        public int Id { get; private set; }
        
        public string Name { get; private set; }
        
        public TargetType Target { get; private set; }
        
        public MappingType MappingType { get; private set; }

        public Dictionary<MappingTargetDeck, string> AssignmentOptions { get; private set; }

        public MappingTargetDeck Assignment
        {
            get { return RawSettings.Target; }
            set { RawSettings.Target = value; }
        }

        public Dictionary<MappingControlType, string> ControlTypeOptions { get; private set; }

        public MappingControlType ControlType
        {
            get { return RawSettings.ControlType; }
            set
            {
                RawSettings.ControlType = value;
             
                updateControlInteractionOptions();
                if (!ControlInteractionOptions.ContainsKey(InteractionMode))
                    RawSettings.InteractionMode = ControlInteractionOptions.First().Key;

                updateControl();
            }
        }

        public Dictionary<MappingInteractionMode, string> ControlInteractionOptions { get; private set; }

        public MappingInteractionMode InteractionMode
        {
            get { return RawSettings.InteractionMode; }
            set { RawSettings.InteractionMode = value; updateControl(); }
        }

        public AControl Control { get; private set; }


        internal ACommand(int id, string name, TargetType target, MappingType mappingType, MappingSettings rawSettings)
        {
            RawSettings = rawSettings;

            Id = id;
            Name = name;
            Target = target;
            MappingType = mappingType;

            updateAssignmentOptions();
            if (!AssignmentOptions.ContainsKey(Assignment))
                RawSettings.Target = AssignmentOptions.First().Key;

            updateControlTypeOptions();
            if (!ControlTypeOptions.ContainsKey(ControlType))
                RawSettings.ControlType = ControlTypeOptions.First().Key;

            updateControlInteractionOptions();
            if (!ControlInteractionOptions.ContainsKey(InteractionMode))
                RawSettings.InteractionMode = ControlInteractionOptions.First().Key;

            updateControl();
        }


        private void updateAssignmentOptions()
        {
            var options = new Dictionary<MappingTargetDeck, string>();

            switch (Target)
            {
                case TargetType.Global:
                    options.Add(MappingTargetDeck.AorFX1orRemixDeck1Slot1OrGlobal, "Global");
                    break;
                case TargetType.Track:
                    options.Add(MappingTargetDeck.DeviceTarget, "Device Target");

                    if ((KnownCommands)Id == KnownCommands.DeckCommon_DeckSizeSelector || (KnownCommands)Id == KnownCommands.DeckCommon_AdvancedPanelToggle)
                    {
                        options.Add(MappingTargetDeck.AorFX1orRemixDeck1Slot1OrGlobal, "Deck A & B");
                        options.Add(MappingTargetDeck.BorFX2orRemixDeck1Slot2, "Deck C & D");
                    }
                    else
                    {
                        options.Add(MappingTargetDeck.AorFX1orRemixDeck1Slot1OrGlobal, "Deck A");
                        options.Add(MappingTargetDeck.BorFX2orRemixDeck1Slot2, "Deck B");
                        options.Add(MappingTargetDeck.CorFX3orRemixDeck1Slot3, "Deck C");
                        options.Add(MappingTargetDeck.DorFX4orRemixDeck1Slot4, "Deck D");
                    }
                    break;
                case TargetType.Remix:
                    options.Add(MappingTargetDeck.DeviceTarget, "Device Target");
                    options.Add(MappingTargetDeck.AorFX1orRemixDeck1Slot1OrGlobal, "Remix Deck 1");
                    options.Add(MappingTargetDeck.BorFX2orRemixDeck1Slot2, "Remix Deck 2");
                    options.Add(MappingTargetDeck.CorFX3orRemixDeck1Slot3, "Remix Deck 3");
                    options.Add(MappingTargetDeck.DorFX4orRemixDeck1Slot4, "Remix Deck 4");
                    break;
                case TargetType.FX:
                    options.Add(MappingTargetDeck.AorFX1orRemixDeck1Slot1OrGlobal, "FX Unit 1");
                    options.Add(MappingTargetDeck.BorFX2orRemixDeck1Slot2, "FX Unit 2");
                    options.Add(MappingTargetDeck.CorFX3orRemixDeck1Slot3, "FX Unit 3");
                    options.Add(MappingTargetDeck.DorFX4orRemixDeck1Slot4, "FX Unit 4");
                    break;
                case TargetType.Slot:
                    options.Add(MappingTargetDeck.AorFX1orRemixDeck1Slot1OrGlobal, "Remix Deck 1 - Slot 1");
                    options.Add(MappingTargetDeck.BorFX2orRemixDeck1Slot2, "Remix Deck 1 - Slot 2");
                    options.Add(MappingTargetDeck.CorFX3orRemixDeck1Slot3, "Remix Deck 1 - Slot 3");
                    options.Add(MappingTargetDeck.DorFX4orRemixDeck1Slot4, "Remix Deck 1 - Slot 4");
                    options.Add(MappingTargetDeck.RemixDeck2Slot1, "Remix Deck 2 - Slot 1");
                    options.Add(MappingTargetDeck.RemixDeck2Slot2, "Remix Deck 2 - Slot 2");
                    options.Add(MappingTargetDeck.RemixDeck2Slot3, "Remix Deck 2 - Slot 3");
                    options.Add(MappingTargetDeck.RemixDeck2Slot4, "Remix Deck 2 - Slot 4");
                    options.Add(MappingTargetDeck.RemixDeck3Slot1, "Remix Deck 3 - Slot 1");
                    options.Add(MappingTargetDeck.RemixDeck3Slot2, "Remix Deck 3 - Slot 2");
                    options.Add(MappingTargetDeck.RemixDeck3Slot3, "Remix Deck 3 - Slot 3");
                    options.Add(MappingTargetDeck.RemixDeck3Slot4, "Remix Deck 3 - Slot 4");
                    options.Add(MappingTargetDeck.RemixDeck4Slot1, "Remix Deck 4 - Slot 1");
                    options.Add(MappingTargetDeck.RemixDeck4Slot2, "Remix Deck 4 - Slot 2");
                    options.Add(MappingTargetDeck.RemixDeck4Slot3, "Remix Deck 4 - Slot 3");
                    options.Add(MappingTargetDeck.RemixDeck4Slot4, "Remix Deck 4 - Slot 4");
                    break;
                default:
                    options = Enum.GetValues(typeof(MappingTargetDeck)).Cast<MappingTargetDeck>().ToDictionary(d => d, d => d.ToString());
                    break;
            }
            AssignmentOptions = options;
        }

        private void updateControlTypeOptions()
        {
            ControlTypeOptions = Controls.All.GetControls(this, AllowedInteractionModes)
                .Select(c => c.Type)
                .Distinct()
                .ToDictionary(t => t, t => t.ToDescriptionString());
        }

        private void updateControlInteractionOptions()
        {
            ControlInteractionOptions = Controls.All.GetControls(this, AllowedInteractionModes)
                .Where(c => c.Type == ControlType)
                .SelectMany(c => c.AllowedInteractionModes)
                .Distinct()
                .ToDictionary(i => i, i => i.ToDescriptionString());
        }

        private void updateControl()
        {
            Control = Controls.All.GetControl(this);
            RawSettings.HasValueUI = HasValueUI;
        }

        public abstract bool HasValueUI { get; }
        protected abstract MappingInteractionMode[] AllowedInteractionModes { get; }
    }
}
