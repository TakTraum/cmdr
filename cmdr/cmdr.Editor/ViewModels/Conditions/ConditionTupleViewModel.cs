using System.Collections.Generic;
using System.Linq;

namespace cmdr.Editor.ViewModels.Conditions
{
    public class ConditionTupleViewModel : ViewModelBase
    {
        private readonly IEnumerable<MappingViewModel> _mappings;


        public int Frequency
        {
            get { return _mappings.Count(); }
        }

        public string Expression
        {
            get { return _mappings.First().Conditions.ToString(); }
        }

        public string Description
        {
            get { return _mappings.First().Conditions.Name; }
            set
            {
                foreach (var ct in _mappings)
                {
                    ct.Conditions.Name = value;
                    ct.UpdateConditionExpression();
                }
                raisePropertyChanged("Description");
            }
        }


        public ConditionTupleViewModel(IEnumerable<MappingViewModel> mappings)
        {
            _mappings = mappings;
        }
    }
}
