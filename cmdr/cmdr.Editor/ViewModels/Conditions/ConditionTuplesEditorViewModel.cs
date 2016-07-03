using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace cmdr.Editor.ViewModels.Conditions
{
    public class ConditionTuplesEditorViewModel : ViewModelBase
    {
        public ObservableCollection<ConditionTupleViewModel> Descriptions { get; private set; }


        public ConditionTuplesEditorViewModel(IEnumerable<MappingViewModel> mappings)
        {
            var conditionTuples = mappings.Select(m => m.Conditions)
                .Where(c => !String.IsNullOrWhiteSpace(c.ToString()))
                .GroupBy(c => c.ToString())
                .Select(t => new ConditionTupleViewModel(t))
                .ToList();
            Descriptions = new ObservableCollection<ConditionTupleViewModel>(conditionTuples);
        }
    }
}
