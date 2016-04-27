using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Format;
using cmdr.TsiLib.Parsers;
using System;
using System.Linq;
using System.Collections.Generic;

namespace cmdr.TsiLib.Commands
{
    public class EffectSelectorOutCommand : EnumOutCommand<Effect>
    {
        private const int TRAKTOR_EFFECTS_COUNT = 43;

        public bool AllEffects
        {
            get { return ControllerRangeMin == 0 && ControllerRangeMax == (Effect)TRAKTOR_EFFECTS_COUNT; }
            set
            {
                if (value)
                {
                    ControllerRangeMin = 0;
                    ControllerRangeMax = (Effect)TRAKTOR_EFFECTS_COUNT;
                }
                else
                    ControllerRangeMin = ControllerRangeMax = 0;
            }
        }


        internal EffectSelectorOutCommand(int id, string name, Enums.TargetType target, MappingSettings rawSettings)
            : base(id, name, target, rawSettings)
        {
            
        }


        protected override Effect GetDefaultControllerRangeMax()
        {
            return (Effect)TRAKTOR_EFFECTS_COUNT;
        }
    }
}
