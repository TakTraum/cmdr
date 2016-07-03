using cmdr.TsiLib.Conditions;
using System.Collections.Generic;

namespace cmdr.Editor.Metadata
{
    public class DeviceMetadata
    {
        public Dictionary<int, MappingMetadata> MappingMetadata { get; set; }
        public Dictionary<ConditionTuple, string> ConditionDescriptions { get; set; }


        public DeviceMetadata()
        {
            MappingMetadata = new Dictionary<int, MappingMetadata>();
            ConditionDescriptions = new Dictionary<ConditionTuple, string>();
        }
    }
}
