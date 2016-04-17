using System;
using System.Collections.Generic;
using System.Linq;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.Commands.Interpretation;

namespace cmdr.TsiLib.Commands
{
    public class All
    {
        static All()
        {
            getKnownCommands();
        }

        public static IReadOnlyDictionary<int, CommandProxy> KnownInCommands;
        public static IReadOnlyDictionary<int, CommandProxy> KnownOutCommands;


        internal static CommandProxy GetCommandProxy(int id, MappingType mappingType)
        {
            switch (mappingType)
            {
                case MappingType.In:
                    if (KnownInCommands.ContainsKey(id))
                        return KnownInCommands[id];
                    break;
                case MappingType.Out:
                    if (KnownOutCommands.ContainsKey(id))
                        return KnownOutCommands[id];
                    break;
                default:
                    break;
            }

            var description = ((KnownCommands)id).GetCommandDescription();
            return new CommandProxy(description, mappingType);
        }

        private static void getKnownCommands()
        {
            var allDescriptions = Enum.GetValues(typeof(KnownCommands)).Cast<KnownCommands>()
                .Select(c => c.GetCommandDescription());

            KnownInCommands = allDescriptions.Where(d => d.InCommandType != null).ToDictionary(d => d.Id, d => new CommandProxy(d, MappingType.In));
            KnownOutCommands = allDescriptions.Where(d => d.OutCommandType != null).ToDictionary(d => d.Id, d => new CommandProxy(d, MappingType.Out));
        }        
    }
}
