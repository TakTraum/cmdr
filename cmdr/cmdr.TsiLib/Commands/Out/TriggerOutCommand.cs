using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Format;

namespace cmdr.TsiLib.Commands
{
    public class TriggerOutCommand : EnumOutCommand<Enums.OnOff>
    {
        internal TriggerOutCommand(int id, string name, TargetType target, MappingSettings rawSettings)
            : base(id, name, target, rawSettings)
        {

        }
    }
}
