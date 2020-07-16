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
            /*
              How to sort enums by their declaration order with custom attributes: 
              see comment #2 of https://stackoverflow.com/questions/25147228/sort-enums-in-declaration-order
             */

            /*
            var allDescriptions2 = Enum
                .GetValues(typeof(KnownCommands))
                .Cast<KnownCommands>()
                .Select(c => c.GetCommandDescription());

            */

            /*
            var b1 = typeof(KnownCommands);
            var b2 = b1.GetFields();
            var b3 = b2.Where(fi => fi.IsStatic);
            var b4 = b3.OrderBy(fi => fi.MetadataToken);

            var c1 = b4.Select(fi => fi.Name);
            var c2 = c1.Select(fi => Enum.Parse(typeof(KnownCommands), fi));
            var c10 = c2.Select(c => ((KnownCommands)c).GetCommandDescription());

            var allDescriptions3 = c10;
            */

            var allDescriptions = typeof(KnownCommands)
                .GetFields()
                .Where(fi => fi.IsStatic)
                .OrderBy(fi => fi.MetadataToken)
                .Select(fi => fi.Name)
                .Select(fi => Enum.Parse(typeof(KnownCommands), fi))
                .Select(c => ((KnownCommands)c).GetCommandDescription());

            KnownInCommands = allDescriptions.Where(d => d.InCommandType != null).ToDictionary(d => d.Id, d => new CommandProxy(d, MappingType.In));
            KnownOutCommands = allDescriptions.Where(d => d.OutCommandType != null).ToDictionary(d => d.Id, d => new CommandProxy(d, MappingType.Out));
        }        
    }
}
