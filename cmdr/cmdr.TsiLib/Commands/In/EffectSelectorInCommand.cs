
using cmdr.TsiLib.Format;

namespace cmdr.TsiLib.Commands
{
    public class EffectSelectorInCommand : EnumInCommand<Effects.All>
    {
        internal EffectSelectorInCommand(int id, string name, Enums.TargetType target, MappingSettings rawSettings)
            : base(id, name, target, rawSettings)
        {

        }
    }
}
