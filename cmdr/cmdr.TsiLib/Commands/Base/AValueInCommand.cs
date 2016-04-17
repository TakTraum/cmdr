using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Format;

namespace cmdr.TsiLib.Commands
{
    public abstract class AValueInCommand<T> : AValueCommand<T>
    {        
        public T Value
        {
            get { return Parser.DecodeValue(RawSettings.SetValueTo); }
            set { RawSettings.SetValueTo = Parser.EncodeValue(value); }
        }


        internal AValueInCommand(int id, string name, Enums.TargetType target, MappingSettings rawSettings)
            : base(id, name, target, MappingType.In, rawSettings)
        {
            RawSettings.ValueUIType = ValueUIType.ComboBox;
        }
    }
}
