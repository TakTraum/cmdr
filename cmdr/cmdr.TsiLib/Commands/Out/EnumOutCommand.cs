using System;
using System.Collections.Generic;
using System.Linq;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Format;
using cmdr.TsiLib.Parsers;

namespace cmdr.TsiLib.Commands
{
    public class EnumOutCommand<T>: AValueOutCommand<T> where T: struct, IConvertible
    {
        private static Dictionary<T, string> _allValues = EnumParser<T>.AllValues.ToDictionary(v => v, v => (v as Enum).ToDescriptionString());
        public static Dictionary<T, string> AllValues
        {
            get { return EnumOutCommand<T>._allValues; }
        }


        internal EnumOutCommand(int id, string name, TargetType target, MappingSettings rawSettings)
            : base(id, name, target, rawSettings)
        {

        }


        private IParser<T> _parser;
        protected override IParser<T> Parser
        {
            get { return _parser ?? (_parser = new EnumParser<T>()); }
        }

        protected override T GetDefaultControllerRangeMin()
        {
            return EnumParser<T>.AllValues.Min();
        }

        protected override T GetDefaultControllerRangeMax()
        {
            return EnumParser<T>.AllValues.Max();
        }
    }
}
