using System;
using cmdr.TsiLib.Ranges;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Format;

namespace cmdr.TsiLib.Commands
{
    public abstract class ANumericValueInCommand<T> : AValueInCommand<T>
    {
        public T MinValue { get; private set; }
        public T MaxValue { get; private set; }


        internal ANumericValueInCommand(int id, string name, TargetType target, ARange<T> range, MappingSettings rawSettings)
            : base(id, name, target, rawSettings)
        {
            if (!typeof(T).IsNumber())
                throw new Exception("T must be a numeric type.");

            MinValue = range.MinValue;
            MaxValue = range.MaxValue;

            RawSettings.ValueUIType = ValueUIType.Slider;
        }
    }
}
