using System;
using System.Collections.Generic;
using System.Linq;
using cmdr.TsiLib.Conditions.Interpretation;

namespace cmdr.TsiLib.Conditions
{
    public class All
    {
        static All()
        {
            getKnownConditions();
        }

        public static IReadOnlyDictionary<int, ConditionProxy> KnownConditions;


        internal static ConditionProxy GetConditionProxy(int id)
        {
            if (KnownConditions.ContainsKey(id))
                return KnownConditions[id];
            
            var description = ((Interpretation.KnownConditions)id).GetConditionDescription();
            return new ConditionProxy(description);
        }

        private static void getKnownConditions()
        {
            var allDescriptions = Enum.GetValues(typeof(KnownConditions)).Cast<KnownConditions>()
                .Select(c => c.GetConditionDescription());

            KnownConditions = allDescriptions.ToDictionary(d => d.Id, d => new ConditionProxy(d));
        }
    }
}
