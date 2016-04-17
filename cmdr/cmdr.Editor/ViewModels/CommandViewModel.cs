using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SettingControlLibrary.SettingTypes;
using System.Windows.Controls;
using cmdr.Editor.Views.CommandViews;
using ChangeTracking;
using System.ComponentModel;
using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;

namespace cmdr.Editor.ViewModels
{
    public class CommandViewModel : ViewModelBase
    {
        private readonly MappingViewModel _mapping;
        private readonly ACommand _command;

        public Dictionary<MappingControlType, string> ControlTypeOptions
        {
            get { return _command.ControlTypeOptions; }
        }

        public MappingControlType ControlType
        {
            get { return _command.ControlType; }
            set { _command.ControlType = value; raisePropertyChanged("ControlType"); raisePropertyChanged("ControlInteractionOptions"); raisePropertyChanged("InteractionMode"); raisePropertyChanged("Control"); updateMapping(); }
        }

        public Dictionary<MappingInteractionMode, string> ControlInteractionOptions
        {
            get { return _command.ControlInteractionOptions; }
        }

        public MappingInteractionMode InteractionMode
        {
            get { return _command.InteractionMode; }
            set { _command.InteractionMode = value; raisePropertyChanged("InteractionMode"); raisePropertyChanged("Control"); updateMapping(); }
        }

        public ControlViewModel Control
        {
            get
            {
                updateContent();
                var cvm = new ControlViewModel(_command.Control);
                cvm.DirtyStateChanged += (s, e) => onValueChanged();
                return cvm;
            }
        }

        private CommandView _settingsContent;
        public CommandView SettingsContent
        {
            get { return _settingsContent; }
            set { _settingsContent = value; raisePropertyChanged("SettingsContent"); }
        }


        public CommandViewModel(MappingViewModel mapping)
        {
            _mapping = mapping;
            _command = mapping.Command;

            updateContent();
        }


        private void updateContent()
        {
            Type t = _command.GetType();
            if (_command.HasValueUI)
            {
                if (t.InheritsOrImplements(typeof(EnumInCommand<>)))
                    SettingsContent = new EnumCommandView(_command);
                else if (t.InheritsOrImplements(typeof(ANumericValueInCommand<>)))
                    SettingsContent = new FloatCommandView(_command);
            }
            else if (_command.ControlType == MappingControlType.LED)
            {
                if (t.InheritsOrImplements(typeof(EnumOutCommand<>)))
                    SettingsContent = new EnumOutCommandView(_command);
                else if (t.InheritsOrImplements(typeof(ANumericValueOutCommand<>)))
                    SettingsContent = new FloatOutCommandView(_command);
            }
            else
                SettingsContent = null;

            if (SettingsContent != null)
                SettingsContent.ValueChanged += (s, e) => onValueChanged();
        }

        private void onValueChanged()
        {
            raisePropertyChanged("Value");
            updateMapping();
        }


        private void updateMapping()
        {
            _mapping.UpdateInteraction();
        }
    }

}
