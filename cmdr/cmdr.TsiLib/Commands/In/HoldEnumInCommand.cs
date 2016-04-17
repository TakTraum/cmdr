using System;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Format;

namespace cmdr.TsiLib.Commands
{
    public class HoldEnumInCommand<T> : EnumInCommand<T> where T : struct, IConvertible
    {
        internal HoldEnumInCommand(int id, string name, Enums.TargetType target, MappingSettings rawSettings)
            : base(id, name, target, rawSettings)
        {

        }


        protected override MappingInteractionMode[] AllowedInteractionModes
        {
            get
            {
                return new[] { MappingInteractionMode.Hold, MappingInteractionMode.Direct, MappingInteractionMode.Relative };
            }
        }
    }
}
