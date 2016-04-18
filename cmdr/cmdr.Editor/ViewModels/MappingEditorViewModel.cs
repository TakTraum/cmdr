using cmdr.Editor.Utils;
using cmdr.Editor.ViewModels.Conditions;
using cmdr.Editor.ViewModels.MidiBinding;
using cmdr.TsiLib.Conditions;
using cmdr.TsiLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private ConditionsEditorViewModel _conditionsEditor;
        public ConditionsEditorViewModel ConditionsEditor { get { return _conditionsEditor; } }

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

            _conditionsEditor = new ConditionsEditorViewModel(_mappingViewModels);
            _isConditionsEnabled = _conditionsEditor != null;

            var count = _mappingViewModels.Count();
            _isAny = count > 0;
            _isMulti = count > 1;
            if (count == 1)
            {
                var mvm = _mappingViewModels.Single();
                _command = new CommandViewModel(mvm);
            }

            _midiBindingEditor = MidiBindingEditorViewModel.BuildEditor(_device, _mappingViewModels);
            _isBindingEnabled = _midiBindingEditor != null;
        }
    }
}
