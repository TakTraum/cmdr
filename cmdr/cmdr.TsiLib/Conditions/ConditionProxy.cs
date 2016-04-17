using System;
using System.Globalization;
using System.Reflection;
using cmdr.TsiLib.Conditions.Interpretation;
using cmdr.TsiLib.Format;
using cmdr.TsiLib.Utils;

namespace cmdr.TsiLib.Conditions
{
    public class ConditionProxy : IMenuProxy
    {
        private static BindingFlags _flags = BindingFlags.NonPublic | BindingFlags.Instance;
        private static CultureInfo _culture = CultureInfo.CurrentCulture;

        private ConditionDescription _description;

        public Categories Category { get { return _description.Category; } }
        public string Name { get { return _description.Name; } }


        internal ConditionProxy(ConditionDescription description)
        {
            _description = description;
        }


        internal ACondition Create(MappingSettings rawSettings, ConditionNumber number)
        {
            var condition = (ACondition)Activator.CreateInstance(_description.ConditionType, _flags, null, new object[] { _description.Id, _description.Name, _description.TargetType, rawSettings, number }, _culture);
            return condition;
        }
    }
}
