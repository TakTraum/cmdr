using cmdr.Editor.Utils;
using cmdr.Editor.ViewModels.Comment;
using cmdr.Editor.ViewModels.Conditions;
using cmdr.Editor.ViewModels.MidiBinding;
using cmdr.Editor.ViewModels.Settings;
using cmdr.TsiLib.Enums;
using cmdr.WpfControls.DropDownButton;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

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
