using System;

namespace cmdr.TsiLib.Conditions.Interpretation
{
    internal struct ConditionDescription
    {
        public int Id;
        public Categories Category;
        public string Name;
        public Enums.TargetType TargetType;
        public Type ConditionType;
    }


    internal class ConditionDescriptionAttribute : Attribute
    {
        public ConditionDescription Description { get; set; }


        public ConditionDescriptionAttribute(Categories categories, string name, Enums.TargetType targetType, Type conditionType)
        {
            Description = new ConditionDescription
            {
                Category = categories,
                Name = name,
                TargetType = targetType,
                ConditionType = conditionType,
            };

            if (!Description.ConditionType.InheritsOrImplements(typeof(ACondition)))
                throw new ArgumentException("Condition Type must inherit from ACondition");
        }
    }

}
