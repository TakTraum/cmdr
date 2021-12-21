using cmdr.Editor.Utils;
using cmdr.TsiLib.Conditions;
using cmdr.WpfControls.DropDownButton;
using cmdr.WpfControls.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace cmdr.Editor.ViewModels.Reports
{
    public class ConditionsEditorViewModel : ViewModelBase
    {
        private static readonly MenuItemViewModel SEPARATOR = MenuItemViewModel.Separator;

        private static List<MenuItemViewModel> _allConditions = null;
        private static readonly MenuBuilder<ConditionProxy> _proxyMenuBuilder = new MenuBuilder<ConditionProxy>();
        private static readonly MenuBuilder<ACondition> _conditionMenuBuilder = new MenuBuilder<ACondition>();
        private static readonly MenuItemViewModel _selectedConditionsSeparator = MenuItemViewModel.Separator;

        private readonly IEnumerable<MappingViewModel> _mappings;
        
        private List<ACondition> _selectedC1Conditions;
        private List<ACondition> _selectedC2Conditions;
        private MenuItemViewModel _selectedConditionsMenuItem = new MenuItemViewModel { Text = "Selected Conditions" };

        public ObservableCollection<MenuItemViewModel> Conditions { get; private set; }
        
        private ConditionViewModel _condition1;
        public ConditionViewModel Condition1
        {
            get { return _condition1; }
            private set { _condition1 = value; raisePropertyChanged("Condition1"); }
        }

        private bool _condition1Various;
        public bool Condition1Various
        {
            get { return _condition1Various; }
            set { _condition1Various = value; raisePropertyChanged("Condition1Various"); }
        }

        private ConditionViewModel _condition2;
        public ConditionViewModel Condition2
        {
            get { return _condition2; }
            private set { _condition2 = value; raisePropertyChanged("Condition2"); }
        }

        private bool _condition2Various;
        public bool Condition2Various
        {
            get { return _condition2Various; }
            set { _condition2Various = value; raisePropertyChanged("Condition2Various"); }
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

            if (_allConditions == null)
                _allConditions = generateConditionsContextMenu();

            Conditions = new ObservableCollection<MenuItemViewModel>(_allConditions);

            Refresh();
        }


        public void Refresh()
        {
            var conditions = _mappings.Select(m => m.Conditions).ToList();
            _selectedC1Conditions = conditions.Select(c => c.Condition1).Distinct().ToList();
            _selectedC2Conditions = conditions.Select(c => c.Condition2).Distinct().ToList();

            update();
        }

        #region Update

        private void update()
        {
            var c1 = getCommonCondition(_selectedC1Conditions);
            Condition1 = new ConditionViewModel(_mappings, ConditionNumber.One, c1);
            Condition1Various = _selectedC1Conditions.Where(c => c != null).Any() && c1 == null;

            var c2 = getCommonCondition(_selectedC2Conditions);
            Condition2 = new ConditionViewModel(_mappings, ConditionNumber.Two, c2);
            Condition2Various = _selectedC2Conditions.Where(c => c != null).Any() && c2 == null;

            updateConditionsMenu(
                _selectedC1Conditions.Where(c => c != null && !c.Equals(c1)),
                _selectedC2Conditions.Where(c => c != null && !c.Equals(c2)));
        }

        private ACondition getCommonCondition(IEnumerable<ACondition> conditions)
        {
            if (conditions.Count() > 1)
                return null;
            return conditions.SingleOrDefault();
        }

        private void updateConditionsMenu(IEnumerable<ACondition> forC1, IEnumerable<ACondition> forC2)
        {
            if (!forC1.Any() && !forC2.Any())
            {
                if (Conditions.Contains(_selectedConditionsMenuItem))
                {
                    Conditions.Remove(_selectedConditionsSeparator);
                    Conditions.Remove(_selectedConditionsMenuItem);
                }
                return;
            }

            var itemBuilder = new Func<ACondition, MenuItemViewModel>(p => new MenuItemViewModel
            {
                Text = p.ToString(),
                IconUri = new Uri(
                    (p.Number == ConditionNumber.One) ?
                    "../Assets/flaticons/number-one-in-a-circle.png" :
                    "../Assets/flaticons/number-two-in-a-circle.png"
                    , UriKind.Relative),
                Tag = p
            });

            var pathSelector = new Func<ACondition, string>(c =>
            {
                if (All.KnownConditions.ContainsKey(c.Id))
                    return All.KnownConditions[c.Id].Category.ToDescriptionString();
                else
                    return "Unknown";
            });

            var tree = _conditionMenuBuilder.BuildTree(forC1.Union(forC2), itemBuilder, pathSelector, "->", false);
            if (tree.Count == 1)
                tree = tree.Single().Children;

            _selectedConditionsMenuItem.Children = tree;

            if (!Conditions.Contains(_selectedConditionsMenuItem))
            {
                Conditions.Add(_selectedConditionsSeparator);
                Conditions.Add(_selectedConditionsMenuItem);
            }
        }

        #endregion

        private void setCondition(ConditionNumber number, MenuItemViewModel item)
        {
            ConditionProxy proxy = null;
            ACondition condition = null;

            if (item.Tag is ConditionProxy)
                proxy = item.Tag as ConditionProxy;
            else if (item.Tag is ACondition)
                condition = item.Tag as ACondition;

            foreach (var mapping in _mappings)
            {
                if (proxy != null)
                    mapping.SetCondition(number, proxy);
                else if (condition != null)
                    mapping.SetCondition(number, condition);
                else
                    mapping.SetCondition(number, condition); // clear condition with null value
            }

            if (number == ConditionNumber.One)
                _selectedC1Conditions = new List<ACondition> { _mappings.First().Conditions.Condition1 };
            else
                _selectedC2Conditions = new List<ACondition> { _mappings.First().Conditions.Condition2 };

            update();
        }

        private static List<MenuItemViewModel> simplifyConditionsContextMenu(List<MenuItemViewModel> items)
        {
            /// simplifiy conditions menu by moving groups to root
            /// 
            /// I'm sure this code can be done in cleanly in LINQ
            var to_simplify = new List<string>() { "FX Unit", "Modifier" };
            foreach (var group in to_simplify)
            {
                int index = items.FindIndex(c => c.Text == group);
                if (index != -1)
                {
                    foreach (var item in items[index].Children)
                    {
                        items.Add(item);
                    }
                    // remove whole group
                    items.RemoveAt(index);
                }
            }

            return items;
        }


        private static List<MenuItemViewModel> generateConditionsContextMenu()
        {
            var all = cmdr.TsiLib.Conditions.All.KnownConditions.Select(kv => kv.Value);

            var itemBuilder = new Func<ConditionProxy, MenuItemViewModel>(p => new MenuItemViewModel
            {
                Text = p.Name,
                Tag = p
            });

            var items = _proxyMenuBuilder.BuildTree(all, itemBuilder, a => a.Category.ToDescriptionString(), "->", false).ToList();

            items.Add(SEPARATOR);
            items.Add(new MenuItemViewModel { Text = "None" });

            items = simplifyConditionsContextMenu(items);
            return items;
        }
    }
}
