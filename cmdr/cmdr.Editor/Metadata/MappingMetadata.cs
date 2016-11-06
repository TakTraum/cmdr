using System.Collections.Generic;

namespace cmdr.Editor.Metadata
{
    public class MappingMetadata
    {
        public bool IsLocked { get; set; }
        public List<string> Tags { get; set; }


        public MappingMetadata()
        {
            Tags = new List<string>();
        }
    }
}
