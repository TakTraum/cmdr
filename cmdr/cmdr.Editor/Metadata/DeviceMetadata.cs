using System.Collections.Generic;

namespace cmdr.Editor.Metadata
{
    public class DeviceMetadata
    {
        public Dictionary<int, MappingMetadata> MappingMetadata { get; set; }


        public DeviceMetadata()
        {
            MappingMetadata = new Dictionary<int, MappingMetadata>();
        }
    }
}
