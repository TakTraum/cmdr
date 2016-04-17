using ChangeTracking;
using cmdr.Editor.Utils;
using cmdr.Editor.ViewModels.MidiBinding;
using cmdr.TsiLib.Conditions;
using cmdr.TsiLib.Enums;
using SettingControlLibrary.SettingTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace cmdr.Editor.ViewModels
{
    public class MappingEditorViewModel : ViewModelBase
    {
        private readonly IEnumerable<MappingViewModel> _mappingViewModels;
        private readonly DeviceViewModel _device;

        private readonly bool _isMulti;
        private readonly bool _isAny;

        public string Comment
        {
            get
            {
                if (_isMulti)
                {
                    var common = _mappingViewModels.Select(m => m.Comment).Distinct();
                    if (common.Count() == 1)
                        return common.Single();
                }
                else if (_isAny)
                    return _mappingViewModels.First().Comment;

                return String.Empty;
            }
            set
            {
                foreach (var mvm in _mappingViewModels)
                    mvm.Comment = value;
                raisePropertyChanged("Comment");
            }
        }
        
        #region Conditions

        private bool _isConditionsEnabled = true;
        public bool IsConditionsEnabled
        {
            get { return _isConditionsEnabled; }
            set { _isConditionsEnabled = value; raisePropertyChanged("IsConditionsEnabled"); }
        }

        private ConditionViewModel _condition1;
        public ConditionViewModel Condition1
        {
            get { return _condition1; }
            private set
            {
                _condition1 = value;
                raisePropertyChanged("Condition1");
                onConditionChanged(ConditionNumber.One, _condition1);
                _condition1.DirtyStateChanged += (s, e) => { if (e)onConditionChanged(ConditionNumber.One, _condition1); };
            }
        }

        private ConditionViewModel _condition2;
        public ConditionViewModel Condition2
        {
            get { return _condition2; }
            private set
            {
                _condition2 = value; 
                raisePropertyChanged("Condition2");
                onConditionChanged(ConditionNumber.Two, _condition2);
                _condition2.DirtyStateChanged += (s, e) => { if (e) onConditionChanged(ConditionNumber.Two, _condition2); };
            }
        }

        private ContextMenu _conditionsMenu;
        public ContextMenu ConditionsMenu
        {
            get { return _conditionsMenu ?? (_conditionsMenu = generateConditionsContextMenu()); }
        }

        private ACondition getCondition(ConditionNumber number)
        {
            if (_mappingViewModels.Any(m => ((number == ConditionNumber.One) ? m.Condition1 : m.Condition2) == null))
                return null;

            if (_isMulti)
            {
                var conditions = _mappingViewModels.Select(m => ((number == ConditionNumber.One) ? m.Condition1 : m.Condition2));
                var conditionIds = conditions.Select(c => c.Id).Distinct();
                if (conditionIds.Count() == 1)
                {
                    var condition = conditions.First().Copy(number);

                    var assignments = conditions.Select(c => c.Assignment).Distinct();
                    if (assignments.Count() == 1)
                        condition.Assignment = assignments.Single();
                    else
                        return null;

                    var values = conditions.Select(c => c.GetValue()).Distinct();
                    if (values.Count() == 1)
                        condition.SetValue(values.Single());
                    else
                        return null;
                    return condition;
                }
            }
            else if (_isAny)
                return ((number == ConditionNumber.One) ? _mappingViewModels.Single().Condition1 : _mappingViewModels.Single().Condition2);

            return null;
        }

        private ContextMenu generateConditionsContextMenu()
        {
            var all = cmdr.TsiLib.Conditions.All.KnownConditions.Select(kv => kv.Value);
            var menu = ContextMenuBuilder<ConditionProxy>.Build(all, setCondition);
            menu.Items.Add(new MenuItem { Header = "None", Command = new CommandHandler(clearCondition) });
            return menu;
        }

        private void showConditions(Button button)
        {
            ConditionsMenu.PlacementTarget = button;
            ConditionsMenu.IsOpen = true;
            ConditionsMenu.Tag = button.Name;
        }

        private void setCondition(ConditionProxy proxy)
        {
            var number = (ConditionsMenu.Tag.ToString() == "btnCondition1") ? ConditionNumber.One : ConditionNumber.Two;

            if (_isMulti)
            {
                foreach (var mvm in _mappingViewModels)
                    mvm.SetCondition(number, proxy);

                if (number == ConditionNumber.One)
                {
                    var conditionVM = new ConditionViewModel(_mappingViewModels.First().Condition1.Copy(number));
                    Condition1 = conditionVM;
                }
                else
                {
                    var conditionVM = new ConditionViewModel(_mappingViewModels.First().Condition2.Copy(number));
                    Condition2 = conditionVM;
                }
            }
            else if (_isAny)
            {
                var mvm = _mappingViewModels.Single();
                mvm.SetCondition(number, proxy);

                if (number == ConditionNumber.One)
                {
                    var conditionVM = new ConditionViewModel(mvm.Condition1);
                    Condition1 = conditionVM;
                }
                else
                {
                    var conditionVM = new ConditionViewModel(mvm.Condition2);
                    Condition2 = conditionVM;
                }
            }
        }

        private void clearCondition()
        {
            if (ConditionsMenu.Tag.ToString() == "btnCondition1")
                Condition1 = new ConditionViewModel(null);
            else
                Condition2 = new ConditionViewModel(null);
        }

        private void onConditionChanged(ConditionNumber number, ConditionViewModel cvm)
        {
            cvm.AcceptChanges();

            foreach (var vm in _mappingViewModels)
            {
                if (cvm.Condition == null)
                {
                    vm.SetCondition(number, null);
                    continue;
                }

                if (number == ConditionNumber.One)
                {
                    vm.Condition1.Assignment = cvm.Condition.Assignment;
                    vm.Condition1.SetValue(cvm.Condition.GetValue());
                }
                else
                {
                    vm.Condition2.Assignment = cvm.Condition.Assignment;
                    vm.Condition2.SetValue(cvm.Condition.GetValue());
                }
                
                vm.UpdateConditions();
            }
        }

        private ICommand _showConditionsCommand;
        public ICommand ShowConditionsCommand
        {
            get { return _showConditionsCommand ?? (_showConditionsCommand = new CommandHandler<Button>(showConditions)); }
        }

        #endregion

        #region Assignment

        public Dictionary<MappingTargetDeck, string> AssignmentOptions
        {
            get
            {
                if (_isAny)
                    return _mappingViewModels.First().Command.AssignmentOptions;
                return new Dictionary<MappingTargetDeck, string>();
            }
        }

        private bool _isAssignmentEnabled;
        public bool IsAssignmentEnabled
        {
            get { return _isAssignmentEnabled; }
            set { _isAssignmentEnabled = value; raisePropertyChanged("IsAssignmentEnabled"); }
        }

        public MappingTargetDeck Assignment
        {
            get
            {
                if (_isMulti)
                {
                    var targetTypes = _mappingViewModels.Select(m => m.TargetType).Distinct();
                    if (targetTypes.Count() == 1)
                    {
                        IsAssignmentEnabled = true;
                        var common = _mappingViewModels.Select(m => m.Assignment).Distinct();
                        if (common.Count() == 1)
                            return common.Single();
                    }
                    else
                        IsAssignmentEnabled = false;
                }
                else if (_isAny)
                {
                    IsAssignmentEnabled = true;
                    return _mappingViewModels.First().Assignment;
                }

                return (MappingTargetDeck)(-1);
            }
            set
            {
                foreach (var mvm in _mappingViewModels)
                    mvm.Assignment = value;
                raisePropertyChanged("Assignment");
            }
        }
        
        #endregion

        #region Control

        public bool IsControlEnabled
        {
            get { return _isAny && !_isMulti; }
        }

        private CommandViewModel _command;
        public CommandViewModel Command { get { return _command; } }

        #endregion

        #region Override Factory Map

        public bool CanOverrideFactoryMap
        {
            get { return _mappingViewModels.Any(m => m.CanOverrideFactoryMap); }
        }

        public bool? OverrideFactoryMap
        {
            get
            {
                if (_isMulti)
                {
                    var common = _mappingViewModels.Select(m => m.OverrideFactoryMap).Distinct();
                    if (common.Count() == 1)
                        return common.Single();
                    else
                        return null;
                }
                else if (_isAny)
                    return _mappingViewModels.First().OverrideFactoryMap;

                return false;
            }
            set
            {
                foreach (var mvm in _mappingViewModels)
                    mvm.OverrideFactoryMap = value.HasValue ? value.Value : false;
                raisePropertyChanged("OverrideFactoryMap");
            }
        }

        #endregion

        #region MidiBinding

        private bool _isBindingEnabled = true;
        public bool IsBindingEnabled
        {
            get { return _isBindingEnabled; }
            set { _isBindingEnabled = value; raisePropertyChanged("IsBindingEnabled"); }
        }

        private MidiBindingEditorViewModel _midiBindingEditor;
        public MidiBindingEditorViewModel MidiBindingEditor { get { return _midiBindingEditor; } }

        #endregion


        public MappingEditorViewModel(DeviceViewModel device, IEnumerable<MappingViewModel> mappingViewModels)
        {
            _device = device;

            _mappingViewModels = mappingViewModels ?? new List<MappingViewModel>();

            var count = _mappingViewModels.Count();
            _isAny = count > 0;
            _isMulti = count > 1;

            if (_mappingViewModels.All(mvm => mvm.Condition1 != null))
            {
                _condition1 = new ConditionViewModel(getCondition(ConditionNumber.One));
                _condition1.DirtyStateChanged += (s, e) => { if (e) onConditionChanged(ConditionNumber.One, _condition1); };
            }

            if (_mappingViewModels.All(mvm => mvm.Condition2 != null))
            {
                _condition2 = new ConditionViewModel(getCondition(ConditionNumber.Two));
                _condition2.DirtyStateChanged += (s, e) => { if (e) onConditionChanged(ConditionNumber.Two, _condition2); };
            }

            if (_isAny && !_isMulti)
            {
                var mvm = _mappingViewModels.Single();
                _command = new CommandViewModel(mvm);
            }

            _midiBindingEditor = MidiBindingEditorViewModel.BuildEditor(_device, _mappingViewModels);
            _isBindingEnabled = _midiBindingEditor != null;
        }
    }
}
