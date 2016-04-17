﻿using ChangeTracking;
using cmdr.TsiLib.Conditions;
using cmdr.TsiLib.Enums;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace cmdr.Editor.ViewModels
{
    public class ConditionAssignment
    {
        public MappingTargetDeck Target { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return Description;
        }
    }

    public class ConditionValue
    {
        public Enum Value { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return Description;
        }
    }

    public class ConditionViewModel : AReversible
    {
        private ACondition _condition;
        public ACondition Condition
        {
            get { return _condition; }
        }

        private string _name;
        public string Name
        {
            get { return _name ?? String.Empty; }
        }

        private ObservableCollection<ConditionAssignment> _assignmentOptions;
        public ObservableCollection<ConditionAssignment> AssignmentOptions
        {
            get { return _assignmentOptions ?? (_assignmentOptions = new ObservableCollection<ConditionAssignment>()); }
        }

        private ConditionAssignment _assignment;
        public ConditionAssignment Assignment
        {
            get { return _assignment;}
            set { _assignment = value; raisePropertyChanged("Assignment"); updateCondition(); IsChanged = true; }
        }

        private ObservableCollection<ConditionValue> _valueOptions;
        public ObservableCollection<ConditionValue> ValueOptions
        {
            get { return _valueOptions ?? (_valueOptions = new ObservableCollection<ConditionValue>()); }
        }

        private ConditionValue _value;
        public ConditionValue Value
        {
            get { return _value; }
            set { _value = value; raisePropertyChanged("Value"); updateCondition(); IsChanged = true; }
        }

        public bool IsNotGlobal
        {
            get { return _condition != null && _condition.Target != TargetType.Global; }
        }


        public ConditionViewModel(ACondition condition)
        {
            _condition = condition;

            if (_condition != null)
            {
                _name = _condition.Name;

                // assignment
                var options = _condition.AssignmentOptions.Select(a => new ConditionAssignment { Target = a.Key, Description = a.Value });
                _assignmentOptions = new ObservableCollection<ConditionAssignment>(options);
                _assignment = AssignmentOptions.SingleOrDefault(a => a.Target == _condition.Assignment);

                // value
                var enumDict = _condition.GetValueOptions();
                _valueOptions = new ObservableCollection<ConditionValue>(enumDict.Select(e => new ConditionValue { Value = e.Key, Description = e.Value }));
                _value = ValueOptions.SingleOrDefault(v => v.Value != null && v.Value.Equals(_condition.GetValue()));
            }

            AcceptChanges();
        }


        protected override void Accept()
        {
            
        }

        protected override void Revert()
        {

        }


        private void updateCondition()
        {
            if (_condition != null)
            {
                _condition.Assignment = Assignment.Target;
                _condition.SetValue(Value.Value);
            }
        }
    }
}
