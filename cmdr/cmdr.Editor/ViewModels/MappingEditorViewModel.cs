using cmdr.Editor.Utils;
using cmdr.Editor.ViewModels.Comment;
using cmdr.Editor.ViewModels.Reports;
using cmdr.Editor.ViewModels.MidiBinding;
using cmdr.Editor.ViewModels.Settings;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Commands;
using cmdr.WpfControls.DropDownButton;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using System;
using System.Text;
using cmdr.TsiLib.Commands.Interpretation;
using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Conditions;
using cmdr.TsiLib.MidiDefinitions.Base;
using cmdr.TsiLib.MidiDefinitions;

namespace cmdr.Editor.ViewModels
{
    public class MappingEditorViewModel : ViewModelBase
    {
        private readonly IEnumerable<MappingViewModel> _mappings;
        private readonly DeviceViewModel _device;

        private CommentEditorViewModel _commentEditor;
        public CommentEditorViewModel CommentEditor { get { return _commentEditor; } }

        private ConditionsEditorViewModel _conditionsEditor;
        public ConditionsEditorViewModel ConditionsEditor { get { return _conditionsEditor; } }


        #region Command

        private bool _isCommandEnabled;
        public bool IsCommandEnabled
        {
            get { return _isCommandEnabled; }
        }

        private CommandEditorViewModel _commandEditor;
        public CommandEditorViewModel CommandEditor { get { return _commandEditor; } }

        #endregion

        #region MidiBinding

        private bool _isBindingEnabled = true;
        public bool IsBindingEnabled
        {
            get { return _isBindingEnabled; }
        }

        private MidiBindingEditorViewModel _midiBindingEditor;
        public MidiBindingEditorViewModel MidiBindingEditor { get { return _midiBindingEditor; } }

        #endregion

        #region Advanced Options

        private ObservableCollection<MenuItemViewModel> _advancedOptions;
        public ObservableCollection<MenuItemViewModel> AdvancedOptions
        {
            get { return _advancedOptions; }
            set { _advancedOptions = value; raisePropertyChanged("AdvancedOptions"); }
        }

        private ICommand _advancedOptionsCommand;
        public ICommand AdvancedOptionsCommand
        {
            get { return _advancedOptionsCommand ?? (_advancedOptionsCommand = new CommandHandler<MenuItemViewModel>(mi => {}, () => AdvancedOptions.Any())); }
        }

        private MenuItemViewModel _changeAssignmentOption;
        private MenuItemViewModel _applyMidiRangeOption;

        #endregion


        public MappingEditorViewModel(DeviceViewModel device, IEnumerable<MappingViewModel> mappings)
        {
            _device = device;

            _mappings = mappings ?? new List<MappingViewModel>();

            _commentEditor = new CommentEditorViewModel(_mappings);

            _conditionsEditor = new ConditionsEditorViewModel(_mappings);
            _conditionsEditor.PropertyChanged += onConditionsChanged;

            _commandEditor = CommandEditorViewModel.BuildEditor(_mappings);
            _isCommandEnabled = _commandEditor != null;
            if (_isCommandEnabled)
                _commandEditor.PropertyChanged += onCommandChanged;

            _midiBindingEditor = MidiBindingEditorViewModel.BuildEditor(_device, _mappings);
            _isBindingEnabled = _midiBindingEditor != null;

            buildAdvancedOptionsMenu();
        }


        private void buildAdvancedOptionsMenu()
        {
            AdvancedOptions = new ObservableCollection<MenuItemViewModel>();

            if (!_mappings.Any())
                return;

            updateChangeAssignmentOption();
            updateApplyMidiRangeOption();
        }

        private void updateChangeAssignmentOption()
        {
            var sameTargetType = _mappings.Select(m => m.TargetType).Distinct();
            if (sameTargetType.Count() == 1 && sameTargetType.Single() != TargetType.Global) 
                {
                var sameAssignment = _mappings.Select(m => m.Assignment).Distinct();
                if (sameAssignment.Count() == 1) 
                    {
                    if (_changeAssignmentOption == null)
                        _changeAssignmentOption = new MenuItemViewModel();

                    if (!AdvancedOptions.Contains(_changeAssignmentOption))
                        AdvancedOptions.Add(_changeAssignmentOption);

                    var assignmentOptions = _mappings.First().Command.AssignmentOptions;
                    var oldAssignment = assignmentOptions.First(ao => ao.Key == sameAssignment.Single());
                    _changeAssignmentOption.Text = "Change Assignments: " + oldAssignment.Value + " ->";
                    _changeAssignmentOption.Children = assignmentOptions
                        .Where(ao => ao.Key != oldAssignment.Key)
                        .Select(o =>
                            new MenuItemViewModel
                            {
                                Text = o.Value,
                                Command = new CommandHandler<MenuItemViewModel>(mi => changeAssignment(o.Key))
                            }
                        ).ToList();

                    return;
                }
            }

            if (AdvancedOptions.Contains(_changeAssignmentOption))
                AdvancedOptions.Remove(_changeAssignmentOption);
        }

        private void changeAssignment(MappingTargetDeck assignment)
        {
            foreach (var m in _mappings)
                m.ChangeAssignment(assignment);

            CommandEditor.Assignment = assignment;
            ConditionsEditor.Refresh();
        }


        public void rotateAssignment(int step)
        {
            foreach (var m in _mappings) 
            {
                var assignmentOptions = m.Command.AssignmentOptions;
                var target =  m.TargetType;

                MappingTargetDeck cur_assignment = m.Assignment;
                MappingTargetDeck new_assignment;
                MappingTargetDeck max_assignment = MappingTargetDeck.DorFX4orRemixDeck1Slot4;

                // Todo: move this to class command
                switch (target) 
                    {
                    case TargetType.Slot:
                        max_assignment = MappingTargetDeck.RemixDeck4Slot4;
                        break;

                    case TargetType.Global:
                        max_assignment = MappingTargetDeck.AorFX1orRemixDeck1Slot1OrGlobal;
                        break;

                    case TargetType.FX:
                    case TargetType.Remix:
                        max_assignment = MappingTargetDeck.DorFX4orRemixDeck1Slot4;
                        break;

                    case TargetType.Track:
                    default:
                        var Id = m.Command.Id;
                        if ((KnownCommands)Id == KnownCommands.DeckCommon_DeckSizeSelector || (KnownCommands)Id == KnownCommands.DeckCommon_AdvancedPanelToggle) {
                            max_assignment = MappingTargetDeck.BorFX2orRemixDeck1Slot2;

                        }
                        else 
                        {
                            max_assignment = MappingTargetDeck.DorFX4orRemixDeck1Slot4;
                        }
                        break;
                }

                new_assignment = cur_assignment;
                if (step > 0) 
                    {
                    if (cur_assignment >= max_assignment) 
                        {
                        new_assignment = MappingTargetDeck.AorFX1orRemixDeck1Slot1OrGlobal;

                    } else 
                    {
                        new_assignment++;
                    }
                } 
                else if (step < 0) 
                    {
                    if (cur_assignment <= MappingTargetDeck.AorFX1orRemixDeck1Slot1OrGlobal) 
                        {
                        new_assignment = max_assignment;
                    } 
                    else 
                    {
                        new_assignment--;
                    }
                }
                m.ChangeAssignment(new_assignment);

            }

            ConditionsEditor.Refresh();

            //analyzeSelection(true);
            //updateMenus(false, true);
        }




        int rotate_modifier_key_int(int cur_modifier, int step, int start, int stop)
        {

            int new_modifier = 1;
            if (step > 0) 
                {
                if (cur_modifier >= stop) 
                    {
                    new_modifier = start;
                } 
                else 
                {
                    new_modifier = cur_modifier + 1;
                }
            } 
            else if (step <0) 
                {
                if (cur_modifier <= start) 
                    {
                    new_modifier = stop;
                } 
                else 
                {
                    new_modifier = cur_modifier - 1;
                }
            }
            return new_modifier;
        }


        int rotate_modifier_value_int(int cur_modifier, int step)
        {
            int new_modifier = 1;
            if (step > 0) 
                {
                if (cur_modifier >= 7) 
                    {
                    new_modifier = 0;
                } 
                else 
                {
                    new_modifier = cur_modifier + 1;
                }
            }
            else if (step < 0) 
                {
                if (cur_modifier <= 0) 
                    {
                    new_modifier = 7;
                } 
                else 
                {
                    new_modifier = cur_modifier - 1;
                }
            }
            return new_modifier;
        }

        // code was based on conditionsEditorViewMode::setCondition()
        public void rotateConditionItself(int which, int step)
        {
            ConditionNumber number;
            if (which == 1)
                number = ConditionNumber.One;
            else
                number = ConditionNumber.Two;

            var conditions_editor = this.ConditionsEditor;
            var conditions_list = conditions_editor.Conditions;

            var first_modifier = conditions_list.FirstOrDefault(x => x.Text == "M1");

            // FIXME: this broken because of the tree removal
            if(first_modifier == null) {
                return;
            };

            int location = conditions_list.IndexOf(first_modifier);
            var modifier_list = conditions_list.Skip(location).Take(8).ToList();

            foreach (var mapping in _mappings) 
{
                ACondition cur_condition;
                if (which == 1)
                    cur_condition = mapping.Conditions.Condition1;
                else
                    cur_condition = mapping.Conditions.Condition2;

                if (cur_condition == null)
                    continue;       // ignore no condition

                KnownCommands id = (KnownCommands)cur_condition.Id;
                if (!((id >= KnownCommands.Modifier_Modifier1) &&
                      (id <= KnownCommands.Modifier_Modifier8)))
                    continue;       // ignore non-modifiers

                var cur_value = cur_condition.GetValue();

                ////
                int cur_modifier = id - KnownCommands.Modifier_Modifier1 + 1;
                int new_modifier = rotate_modifier_key_int(cur_modifier, step, 1, 8);

                MenuItemViewModel item = modifier_list[new_modifier - 1];
                ConditionProxy new_proxy = null;
                ACondition new_condition = null;

                if (item.Tag is ConditionProxy)
                    new_proxy = item.Tag as ConditionProxy;
                else if (item.Tag is ACondition)
                    new_condition = item.Tag as ACondition;

                if (new_proxy != null)
                    mapping.SetCondition(number, new_proxy);
                else if (new_condition != null)
                    mapping.SetCondition(number, new_condition);
                else
                    mapping.SetCondition(number, new_condition); // clear condition with null value


                cur_condition.SetValue(cur_value);
                mapping.UpdateConditionExpression();
            }
            conditions_editor.Refresh();
        }


    
        public void rotateConditionValue(int which, int step)
        {
            var conditions_editor = this.ConditionsEditor;
            var conditions_list = conditions_editor.Conditions;
            var modifier_list = conditions_list[2].Children;

            foreach (var mapping in _mappings)
            {
                ACondition cur_condition;
                if (which == 1)
                    cur_condition = mapping.Conditions.Condition1;
                else
                    cur_condition = mapping.Conditions.Condition2;

                if (cur_condition == null)
                    continue;       // ignore no condition

                try_rotate_condition_value(cur_condition, step);


                mapping.UpdateConditionExpression();
            }
            conditions_editor.Refresh();
        }


        // fixme: changes to assigment change conditions as well
        public void try_rotate_condition_value(ACondition command, int step)
        {
            Type type = command.GetType();
            string type_name = type.FullName;

            //if (command.MappingType == MappingType.Out) {
            //    return;
            //}

            if (false) {
                // This is just a placeholder

                ////// Start of Auto generated code
            } else if (type_name.Contains("Enums.CaptureSource")) {
                var command2 = (EnumCondition<CaptureSource>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.CueLoopMoveMode")) {
                var command2 = (EnumCondition<CueLoopMoveMode>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.DeckFlavor")) {
                var command2 = (EnumCondition<DeckFlavor>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.FXUnitMode")) {
                var command2 = (EnumCondition<FXUnitMode>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.HotcueType")) {
                var command2 = (EnumCondition<HotcueType>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.ModifierValue")) {
                var command2 = (EnumCondition<ModifierValue>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.OnOff")) {
                var command2 = (EnumCondition<OnOff>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.PlayMode")) {
                var command2 = (EnumCondition<PlayMode>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.Sample")) {
                var command2 = (EnumCondition<Sample>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.SamplePage")) {
                var command2 = (EnumCondition<SamplePage>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.SlotCellState")) {
                var command2 = (EnumCondition<SlotCellState>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.SlotState")) {
                var command2 = (EnumCondition<SlotState>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.SlotTriggerType")) {
                var command2 = (EnumCondition<SlotTriggerType>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.TempoRange")) {
                var command2 = (EnumCondition<TempoRange>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            }
            ////// End of Auto generated code

        }


        ///////////
        ///////////
        ///////////
        public bool is_range_id(KnownCommands cur_id, KnownCommands start, KnownCommands end)
        {
            return ((cur_id >= start) &&
                    (cur_id <= end)
                    );

        }

        public string patch_command_name(string text, int new_int)
        {
            string ret = "";
            bool done_replace = false;
            var sb = new    StringBuilder();
            for (var i = 0; i < text.Length - 0; i++) 
                {
                if (char.IsDigit(text[i])) 
                {
                    if (!done_replace) 
                    {
                        string to_add = string.Format("{0}", new_int);
                        sb.Append(to_add);

                    }
                    done_replace = true;
                } 
                else 
                {
                    sb.Append(text[i]);
                }
            }

            return sb.ToString();
        }

        public void try_rotate_command(MappingViewModel m, int step, KnownCommands start_c, KnownCommands end_c)
        {
            var command = m.Command;
            KnownCommands cur_id = (KnownCommands)m.Command.Id;

            if (!is_range_id(cur_id, start_c, end_c)){
                return;
            }

            int cur_modifier = cur_id - start_c + 1;

            int start_i = 1;
            int end_i = 8;

            switch (end_c) 
            {
                case KnownCommands.FXUnit_Button3:
                case KnownCommands.FXUnit_Effect3Selector:
                case KnownCommands.FXUnit_Knob3:
                    end_i = 3;
                    break;


                case KnownCommands.Global_MidiControls_Buttons_MidiButton8:
                case KnownCommands.Global_MidiControls_Knobs_MidiFader8:
                case KnownCommands.Global_MidiControls_Knobs_MidiKnob8:
                case KnownCommands.Modifier_Modifier8:
                case KnownCommands.TrackDeck_Cue_Hotcue8Type:
                    end_i = 8;
                    break;

                case KnownCommands.Mixer_FXUnit4On:
                    end_i = 4;
                    break;

                case KnownCommands.RemixDeck_StepSequencer_EnableStep16:
                    // needs bugfix?
                    //return;

                    end_i = 16;
                    break;
            }

            int new_modifier = rotate_modifier_key_int(cur_modifier, step, start_i, end_i);

            string new_name = patch_command_name(command.Name, new_modifier);

            KnownCommands new_id = start_c + new_modifier - 1;

            // pestrela: this is a bit fragile, and should be improved
            m.Command.hack_modifier(new_id, new_name);
            m.hack_modifier(new_id);

            // would this be the proper way?
            //    var _rawMapping = m._mapping.RawMapping;
            //    ACommand Command = Commands.All.GetCommandProxy(_rawMapping.TraktorControlId, _rawMapping.Type).Create(_rawMapping.Settings);

            m.UpdateInteraction();
        }

        public List<KnownCommands> get_rotatable_commands() {

            KnownCommands[] array = {
                    /* 
                     * ignored:
                            KnownCommands.RemixDeck_DirectMapping_Slot4_Slot4Cell13State
                            KnownCommands.RemixDeck_DirectMapping_Slot4_Slot4Cell13Trigger
                    */
                    KnownCommands.DeckCommon_FreezeMode_SliceTrigger1,
                    KnownCommands.DeckCommon_FreezeMode_SliceTrigger16,

                    KnownCommands.FXUnit_Button1,
                    KnownCommands.FXUnit_Button3,
                    KnownCommands.FXUnit_Effect1Selector,
                    KnownCommands.FXUnit_Effect3Selector,
                    KnownCommands.FXUnit_Knob1,
                    KnownCommands.FXUnit_Knob3,

                    KnownCommands.Global_MidiControls_Buttons_MidiButton1,
                    KnownCommands.Global_MidiControls_Buttons_MidiButton8,
                    KnownCommands.Global_MidiControls_Knobs_MidiFader1,
                    KnownCommands.Global_MidiControls_Knobs_MidiFader8,
                    KnownCommands.Global_MidiControls_Knobs_MidiKnob1,
                    KnownCommands.Global_MidiControls_Knobs_MidiKnob8,

                    KnownCommands.Mixer_FXUnit1On,
                    KnownCommands.Mixer_FXUnit4On,
                    KnownCommands.Modifier_Modifier1,
                    KnownCommands.Modifier_Modifier8,

                    KnownCommands.RemixDeck_StepSequencer_EnableStep1,
                    KnownCommands.RemixDeck_StepSequencer_EnableStep16,
                    KnownCommands.TrackDeck_Cue_Hotcue1Type,
                    KnownCommands.TrackDeck_Cue_Hotcue8Type,
                };

            List<KnownCommands> list = new List<KnownCommands>(array);
            return list;
        }


        public void rotateCommandItself(int step)
        {
            var possibilities = get_rotatable_commands();

            foreach (var m in _mappings) 
            {
                // take elements two-by-two
                for (var i = 0; i < possibilities.Count(); i = i + 2) 
                {
                    try_rotate_command(m, step, possibilities[i], possibilities[i + 1]);
                }
            }
        }

        //////////
        //////////
        //////////
        //////////


        public void try_rotate_command_value(ACommand command, int step)
        {
            Type type = command.GetType();
            string type_name = type.FullName;

            if (command.MappingType == MappingType.Out) {
                return;
            }

            // BELOW ITS A HACK!
            // could not put this to work: t.InheritsOrImplements(typeof(EnumInCommand<HotcueType>)))
            // plus many other things

            // special types:
            //  null
            //  typeof(TriggerInCommand)
            //  typeof(HoldInCommand)
            //
            //  typeof(EffectSelectorInCommand)
            //  typeof(OnOffInCommand)

            if (false) {
                // This is just a placeholder

            } else if (type_name.Contains("OnOffInCommand")) {
                var command2 = (OnOffInCommand)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("EffectSelectorInCommand")) {
                var command2 = (EffectSelectorInCommand)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);


                ////// Start of Auto generated code
            } else if (type_name.Contains("Enums.AdvancedPanelTab")) {
                var command2 = (EnumInCommand<AdvancedPanelTab>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.CaptureSource")) {
                var command2 = (EnumInCommand<CaptureSource>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.CuePointOrLoopMoveSize")) {
                var command2 = (EnumInCommand<CuePointOrLoopMoveSize>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.CueType")) {
                var command2 = (EnumInCommand<CueType>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.Deck")) {
                var command2 = (EnumInCommand<Deck>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.DeckFlavor")) {
                var command2 = (EnumInCommand<DeckFlavor>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.DeckSize")) {
                var command2 = (EnumInCommand<DeckSize>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.ExpandCollapse")) {
                var command2 = (EnumInCommand<ExpandCollapse>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.FXRouting")) {
                var command2 = (EnumInCommand<FXRouting>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.FXUnitMode")) {
                var command2 = (EnumInCommand<FXUnitMode>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.Favorite")) {
                var command2 = (EnumInCommand<Favorite>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.FreezeSliceCount")) {
                var command2 = (EnumInCommand<FreezeSliceCount>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.FreezeSliceSize")) {
                var command2 = (EnumInCommand<FreezeSliceSize>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.Hotcue")) {
                var command2 = (EnumInCommand<Hotcue>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.IntExt")) {
                var command2 = (EnumInCommand<IntExt>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.JumpDirection")) {
                var command2 = (EnumInCommand<JumpDirection>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.Layout")) {
                var command2 = (EnumInCommand<Layout>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.ListNavigation")) {
                var command2 = (EnumInCommand<ListNavigation>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.ListOffset")) {
                var command2 = (EnumInCommand<ListOffset>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.LoopRecorderSize")) {
                var command2 = (EnumInCommand<LoopRecorderSize>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.LoopSize")) {
                var command2 = (EnumInCommand<LoopSize>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.MixerFx")) {
                var command2 = (EnumInCommand<MixerFx>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.ModifierValue")) {
                var command2 = (EnumInCommand<ModifierValue>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.MoveDirection")) {
                var command2 = (EnumInCommand<MoveDirection>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.MoveMode")) {
                var command2 = (EnumInCommand<MoveMode>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.MoveSize")) {
                var command2 = (EnumInCommand<MoveSize>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.PatternLength")) {
                var command2 = (EnumInCommand<PatternLength>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.PlatterScopeView")) {
                var command2 = (EnumInCommand<PlatterScopeView>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.PlayMode")) {
                var command2 = (EnumInCommand<PlayMode>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.PlaybackMode")) {
                var command2 = (EnumInCommand<PlaybackMode>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.QuantizeSize")) {
                var command2 = (EnumInCommand<QuantizeSize>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.Sample")) {
                var command2 = (EnumInCommand<Sample>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.SamplePage")) {
                var command2 = (EnumInCommand<SamplePage>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.ScratchControl")) {
                var command2 = (EnumInCommand<ScratchControl>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.SlotCell")) {
                var command2 = (EnumInCommand<SlotCell>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.SlotSize")) {
                var command2 = (EnumInCommand<SlotSize>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.SlotTriggerType")) {
                var command2 = (EnumInCommand<SlotTriggerType>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.TempoRange")) {
                var command2 = (EnumInCommand<TempoRange>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.TempoSource")) {
                var command2 = (EnumInCommand<TempoSource>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.TopBottom")) {
                var command2 = (EnumInCommand<TopBottom>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.TreeNavigation")) {
                var command2 = (EnumInCommand<TreeNavigation>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            } else if (type_name.Contains("Enums.UpDown")) {
                var command2 = (EnumInCommand<UpDown>)command;
                var cur_value = command2.Value;
                command2.Value = cur_value.EnumRotate(step);

            }
            ////// End of Auto generated code

        }

        public void rotateCommandValue(int step)
        {
            foreach (var m in _mappings) {
                try_rotate_command_value(m.Command, step);
                m.UpdateInteraction();

            }
        }
        //////////
        //////////
        //////////


        public void swapConditions()
        {
            var conditions_editor = this.ConditionsEditor;
            var conditions_list = conditions_editor.Conditions;

            foreach (var mapping in _mappings) 
{
                mapping.Conditions.Swap();

                mapping.UpdateConditionExpression();
            }
            conditions_editor.Refresh();
        }


        private void updateApplyMidiRangeOption()
        {
            if (_mappings.Count() > 1 && MidiBindingEditor.ApplyMidiRangeCommand.CanExecute(null)) 
{
                if (_applyMidiRangeOption == null)
                    _applyMidiRangeOption = new MenuItemViewModel();

                if (!AdvancedOptions.Contains(_applyMidiRangeOption))
                    AdvancedOptions.Add(_applyMidiRangeOption);

                _applyMidiRangeOption.Text = "Apply Midi Range";
                _applyMidiRangeOption.Command = MidiBindingEditor.ApplyMidiRangeCommand;

                return;
            }

            if (AdvancedOptions.Contains(_applyMidiRangeOption))
                AdvancedOptions.Remove(_applyMidiRangeOption);
        }

        #region Events

        void onCommandChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Assignment")
                updateChangeAssignmentOption();
        }

        void onConditionsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Condition1" || e.PropertyName == "Condition2")
                updateChangeAssignmentOption();
        }

        #endregion
    }
}
