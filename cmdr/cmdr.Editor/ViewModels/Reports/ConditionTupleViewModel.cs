using cmdr.TsiLib.Conditions;
using System.Collections.Generic;
using System.Linq;


namespace cmdr.Editor.ViewModels.Reports
{
    public class ConditionTupleViewModel : ViewModelBase
    {
        private readonly List<MappingViewModel> _mappings;
        private readonly ConditionTuple _conditionTuple;

        public int Frequency
        {
            get
            {
                return _mappings.Count();
            }
        }

        public string Expression
        {
            get
            {
                return _conditionTuple.ToString();
            }
        }

        public string Description
        {
            get
            {
                return _conditionTuple.Name;
            }
            set
            {
                return;

                // this code is not finished yet
                foreach (var m in _mappings)
                {
                    m.Conditions.Name = value;
                    m.UpdateConditionExpression();
                }
                raisePropertyChanged("Description");
            }
        }


        public ConditionTupleViewModel(List<MappingViewModel> mappings)
        {
            _mappings = mappings;
            _conditionTuple = _mappings.First().Conditions;
        }
    }
}
