using cmdr.Editor.Utils;
using cmdr.TsiLib.Conditions;
using cmdr.WpfControls.DropDownButton;
using cmdr.WpfControls.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private static ObservableCollection<MenuItemViewModel> _conditions;
        public static ObservableCollection<MenuItemViewModel> Conditions
        {
            get { return _conditions ?? (_conditions = new ObservableCollection<MenuItemViewModel>(generateConditionsContextMenu())); }
        }


        private ConditionViewModel _condition1;
        public ConditionViewModel Condition1
        {
            get { return _condition1; }
            private set { _condition1 = value; raisePropertyChanged("Condition1"); }
        }

        private ConditionViewModel _condition2;
        public ConditionViewModel Condition2
        {
            get { return _condition2; }
            private set { _condition2 = value; raisePropertyChanged("Condition2"); }
        }


        #region Commands

        private ICommand _setCondition1Command;
        public ICommand SetCondition1Command
        {
            get { return _setCondition1Command ?? (_setCondition1Command = new CommandHandler<MenuItemViewModel>(item => setCondition(ConditionNumber.One, item))); }
        }

        private ICommand _setCondition2Command;
        public ICommand SetCondition2Command
        {
            get { return _setCondition2Command ?? (_setCondition2Command = new CommandHandler<MenuItemViewModel>(item => setCondition(ConditionNumber.Two, item))); }
        }

        #endregion


        public ConditionsEditorViewModel(IEnumerable<MappingViewModel> mappings)
        {
            _mappings = mappings;

            if (_mappings.All(mvm => mvm.Condition1 != null))
                _condition1 = new ConditionViewModel(_mappings, ConditionNumber.One, getCommonCondition(_mappings, ConditionNumber.One));

            if (_mappings.All(mvm => mvm.Condition2 != null))
                _condition2 = new ConditionViewModel(_mappings, ConditionNumber.Two, getCommonCondition(_mappings, ConditionNumber.Two));
        }


        private void setCondition(ConditionNumber number, MenuItemViewModel item)
        {
            var proxy = item.Tag as ConditionProxy;

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


        private static IEnumerable<MenuItemViewModel> generateConditionsContextMenu()
        {
            var all = cmdr.TsiLib.Conditions.All.KnownConditions.Select(kv => kv.Value);

            var builder = new MenuBuilder<ConditionProxy>();
            var itemBuilder = new Func<ConditionProxy, MenuItemViewModel>(p => new MenuItemViewModel
            {
                Text = p.Name,
                Tag = p
            });

            var items = builder.BuildTree(all, itemBuilder, a => a.Category.ToDescriptionString(), "->", false).ToList();
            items.Add(new MenuItemViewModel { Text = "None" });
            return items;
        }
    }
}
