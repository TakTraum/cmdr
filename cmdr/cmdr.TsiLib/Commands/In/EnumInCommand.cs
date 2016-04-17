using System;
using System.Collections.Generic;
using System.Linq;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Format;
using cmdr.TsiLib.Parsers;

namespace cmdr.TsiLib.Commands
{
    public class EnumInCommand<T> : AValueInCommand<T> where T : struct, IConvertible
    {
        private static Dictionary<T, string> _allValues = EnumParser<T>.AllValues.ToDictionary(v => v, v => (v as Enum).ToDescriptionString());
        public static Dictionary<T, string> AllValues
        {
            get { return EnumInCommand<T>._allValues; }
        }


        internal EnumInCommand(int id, string name, Enums.TargetType target, MappingSettings rawSettings)
            : base(id, name, target, rawSettings)
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
                throw new Exception("T must be an Enumeration type.");

            RawSettings.ValueUIType = ValueUIType.ComboBox;
        }


        public T GetDefaultValue()
        {
            return EnumParser<T>.AllValues.Min();
        }


        protected override MappingInteractionMode[] AllowedInteractionModes
        {
            get
            {
                return new[] { 
                    MappingInteractionMode.Hold,
                    MappingInteractionMode.Direct, 
                    MappingInteractionMode.Relative,
                    MappingInteractionMode.Increment, 
                    MappingInteractionMode.Decrement, 
                    MappingInteractionMode.Reset,
                    };
            }
        }

        private IParser<T> _parser;
        protected override IParser<T> Parser
        {
            get { return _parser ?? (_parser = new EnumParser<T>()); }
        }
    }
}
