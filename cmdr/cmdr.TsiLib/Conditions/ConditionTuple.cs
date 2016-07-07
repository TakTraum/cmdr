using System;
using System.Linq;

namespace cmdr.TsiLib.Conditions
{
    public class ConditionTuple
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private ACondition _condition1;
        public ACondition Condition1
        {
            get { return _condition1; }
            private set
            {
                if (value == null && _condition1 != null)
                    _condition1.Reset();
                _condition1 = value;
            }
        }

        private ACondition _condition2;
        public ACondition Condition2
        {
            get { return _condition2; }
            private set
            {
                if (value == null && _condition2 != null)
                    _condition2.Reset();
                _condition2 = value;
            }
        }


        public ConditionTuple()
        {

        }

        public ConditionTuple(ACondition c1, ACondition c2)
        {
            Condition1 = c1;
            Condition2 = c2;
        }


        public void Swap()
        {
            Format.MappingSettings rawSettings = (Condition1 != null) ? Condition1.RawSettings : (Condition2 != null) ? Condition2.RawSettings : null;
            if (rawSettings == null)
                return;

            var swap = Condition1;
            SetCondition(rawSettings, ConditionNumber.One, Condition2);
            SetCondition(rawSettings, ConditionNumber.Two, swap);           
        }

        public override string ToString()
        {
            var conditions = new[] { Condition1, Condition2 }.Where(c => c != null).OrderBy(c => c.ToString());
            return String.Join(" AND ", conditions.Select(c => c.ToString()));
        } 

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            ConditionTuple other = obj as ConditionTuple;
            if (other == null)
                throw new ArgumentException("object must be a Tuple<ACondition, ACondition>");

            // order doesn't matter

            var containsItem1 = false;
            if (Condition1 != null)
                containsItem1 = Condition1.Equals(other.Condition1) || Condition1.Equals(other.Condition2);
            else
                containsItem1 = other.Condition1 == null || other.Condition2 == null;

            var containsItem2 = false;
            if (Condition2 != null)
                containsItem2 = Condition2.Equals(other.Condition2) || Condition2.Equals(other.Condition1);
            else
                containsItem2 = other.Condition2 == null || other.Condition1 == null;

            return containsItem1 && containsItem2;
        }

        public override int GetHashCode()
        {
            return new { Condition1, Condition2 }.GetHashCode();
        }


        /// <summary>
        /// Set condition by proxy.
        /// </summary>
        /// <param name="mappingSettings">Format.MappingSettings needed by proxy for creation of condition</param>
        /// <param name="number">Number of condition</param>
        /// <param name="proxy">ConditionProxy or null to reset condition.</param>
        /// <returns>True if condition was changed.</returns>
        internal bool SetCondition(Format.MappingSettings mappingSettings, ConditionNumber number, ConditionProxy proxy)
        {
            ACondition condition = (proxy != null) ? proxy.Create(mappingSettings, number) : null;
            if (condition != null)
            {
                condition.Assignment = condition.AssignmentOptions.Keys.First();
                condition.SetValue(condition.GetValueOptions().Keys.First());
            }
            return setCondition(number, condition);
        }

        /// <summary>
        /// Set condition by another condition.
        /// </summary>
        /// <param name="mappingSettings">Format.MappingSettings needed by proxy for creation of condition</param>
        /// <param name="number">Number of condition</param>
        /// <param name="condition">ACondition or null to reset condition.</param>
        /// <returns>True if condition was changed.</returns>
        internal bool SetCondition(Format.MappingSettings mappingSettings, ConditionNumber number, ACondition condition)
        {
            if (condition == null)
                return setCondition(number, condition);
            else
            {
                bool changed = false;

                var targetCondition = (number == ConditionNumber.One) ? Condition1 : Condition2;

                if (targetCondition == null || targetCondition.Id != condition.Id)
                {
                    changed = SetCondition(mappingSettings, number, Conditions.All.GetConditionProxy(condition.Id));
                    targetCondition = (number == ConditionNumber.One) ? Condition1 : Condition2; // needed?
                }

                // set assignment
                if (!changed)
                    changed |= targetCondition.Assignment != condition.Assignment;
                targetCondition.Assignment = condition.Assignment;

                // set value
                var newValue = condition.GetValue();
                if (!changed)
                    changed |= targetCondition.GetValue() != newValue;
                targetCondition.SetValue(newValue);

                return changed;
            }
        }


        /// This one is dumb and must not be called from other classes. RawMapping.Settings are ignored!
        private bool setCondition(ConditionNumber number, ACondition condition)
        {
            bool changed = false;

            if (number == ConditionNumber.One)
            {
                changed = (Condition1 != null && !Condition1.Equals(condition)) || (condition != null && !condition.Equals(Condition1));
                if (changed)
                    Condition1 = condition;
            }
            else
            {
                changed = (Condition2 != null && !Condition2.Equals(condition)) || (condition != null && !condition.Equals(Condition2));
                if (changed)
                    Condition2 = condition;
            }

            return changed;
        }
    }
}
