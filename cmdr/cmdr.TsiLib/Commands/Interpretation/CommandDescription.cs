using System;

namespace cmdr.TsiLib.Commands.Interpretation
{
    internal struct CommandDescription
    {
        public int Id;
        public Categories Category;
        public string Name;
        public Enums.TargetType TargetType;
        public Type InCommandType;
        public Type OutCommandType;
    }


    internal class CommandDescriptionAttribute : Attribute
    {
        public CommandDescription Description { get; set; }


        public CommandDescriptionAttribute(Categories categories, string name, Enums.TargetType targetType, Type inCommandType, Type outCommandType)
        {
            Description = new CommandDescription
            {
                Category = categories,
                Name = name,
                TargetType = targetType,
                InCommandType = inCommandType,
                OutCommandType = outCommandType,
            };

            if (Description.InCommandType != null && !Description.InCommandType.InheritsOrImplements(typeof(ACommand)))
                throw new ArgumentException("In Command Type must inherit from ACommand");

            if (Description.OutCommandType != null && !Description.OutCommandType.InheritsOrImplements(typeof(ACommand)))
                throw new ArgumentException("Out Command Type must inherit from ACommand");
        }
    }
}
