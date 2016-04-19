using cmdr.Editor.Utils;
using cmdr.TsiLib.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace cmdr.Editor.ViewModels.Conditions
{
    public class ConditionsEditorViewModel : ViewModelBase
    {
        private readonly IEnumerable<MappingViewModel> _mappings;


        private ConditionViewModel _condition1;
        public ConditionViewModel Condition1
        {
            get { return _condition1; }
            private set
            {
                _condition1 = value;
                raisePropertyChanged("Condition1");
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
            }
        }

        private ICommand _showConditionsCommand;
        public ICommand ShowConditionsCommand
        {
            get { return _showConditionsCommand ?? (_showConditionsCommand = new CommandHandler<Button>(showConditions)); }
        }

        private ContextMenu _conditionsMenu;
        public ContextMenu ConditionsMenu
        {
            get { return _conditionsMenu ?? (_conditionsMenu = generateConditionsContextMenu()); }
        }


        public ConditionsEditorViewModel(IEnumerable<MappingViewModel> mappings)
        {
            _mappings = mappings;

            if (_mappings.All(mvm => mvm.Condition1 != null))
                _condition1 = new ConditionViewModel(_mappings, ConditionNumber.One, getCommonCondition(_mappings, ConditionNumber.One));

            if (_mappings.All(mvm => mvm.Condition2 != null))
                _condition2 = new ConditionViewModel(_mappings, ConditionNumber.Two, getCommonCondition(_mappings, ConditionNumber.Two));
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

        private void clearCondition()
        {
            setCondition(null);
        }

        private void setCondition(ConditionProxy proxy)
        {
            var number = (ConditionsMenu.Tag.ToString() == "btnCondition1") ? ConditionNumber.One : ConditionNumber.Two;

            foreach (var mapping in _mappings)
                mapping.SetCondition(number, proxy);

            if (number == ConditionNumber.One)
                Condition1 = new ConditionViewModel(_mappings, ConditionNumber.One, _mappings.First().Condition1);
            else
                Condition2 = new ConditionViewModel(_mappings, ConditionNumber.Two, _mappings.First().Condition2);
        }

        private ACondition getCommonCondition(IEnumerable<MappingViewModel> mappings, ConditionNumber number)
        {
            var conditions = mappings.Select(m => ((number == ConditionNumber.One) ? m.Condition1 : m.Condition2)).Distinct();
            if (conditions.Count() > 1)
                return null;
            return conditions.SingleOrDefault();
        }

    }
}
