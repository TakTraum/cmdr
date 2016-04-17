using System;
using cmdr.TsiLib.Ranges;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Format;
using cmdr.TsiLib.Parsers;

namespace cmdr.TsiLib.Commands
{
    public class IntInCommand<T> : ANumericValueInCommand<Int32> where T : IntRange, new()
    {
        internal IntInCommand(int id, string name, Enums.TargetType target, MappingSettings rawSettings)
            : base(id, name, target, new T(), rawSettings)
        {

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

        private IParser<int> _parser;
        protected override IParser<int> Parser
        {
            get { return _parser ?? (_parser = new IntParser()); }
        }
    }
}
