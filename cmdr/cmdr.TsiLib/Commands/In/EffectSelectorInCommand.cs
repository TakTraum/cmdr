using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Format;
using cmdr.TsiLib.Parsers;
using System.Linq;
using System.Collections.Generic;
using System;

namespace cmdr.TsiLib.Commands
{
    public class EffectSelectorInCommand : EnumInCommand<Effect>
    {
        internal EffectSelectorInCommand(int id, string name, Enums.TargetType target, MappingSettings rawSettings)
            : base(id, name, target, rawSettings)
        {

        }
    }
}
