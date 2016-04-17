using cmdr.TsiLib.Ranges;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Format;

namespace cmdr.TsiLib.Commands
{
    public class UnknownOutCommand : IntOutCommand<IntRangeUnlimited>
    {
        internal UnknownOutCommand(int id, string name, TargetType target, MappingSettings rawSettings)
            : base(id, name, target, rawSettings)
        {

        }
    }
}
