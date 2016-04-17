using cmdr.TsiLib.Format;

namespace cmdr.TsiLib.Commands
{
    public class EffectSelectorOutCommand : EnumOutCommand<Effects.All>
    {
        internal EffectSelectorOutCommand(int id, string name, Enums.TargetType target, MappingSettings rawSettings)
            : base(id, name, target, rawSettings)
        {

        }
    }
}
