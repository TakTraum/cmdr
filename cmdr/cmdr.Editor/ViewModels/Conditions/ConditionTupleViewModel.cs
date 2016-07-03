using cmdr.TsiLib.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdr.Editor.ViewModels.Conditions
{
    public class ConditionTupleViewModel : ViewModelBase
    {
        private readonly IEnumerable<ConditionTuple> _conditionTuples;


        public int Frequency
        {
            get { return _conditionTuples.Count(); }
        }

        public string Expression
        {
            get { return _conditionTuples.First().ToString(); }
        }

        public string Description
        {
            get { return _conditionTuples.First().Name; }
            set
            {
                foreach (var ct in _conditionTuples)
                    ct.Name = value;
                raisePropertyChanged("Description");
            }
        }


        public ConditionTupleViewModel(IEnumerable<ConditionTuple> conditionTuples)
        {
            _conditionTuples = conditionTuples;
        }
    }
}
