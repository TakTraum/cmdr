using System.Collections.ObjectModel;

namespace cmdr.Editor.Metadata
{
    public class MappingMetadata
    {
        public bool IsLocked { get; set; }
        public ObservableCollection<string> Tags { get; set; }


        public MappingMetadata()
        {
            Tags = new ObservableCollection<string>();
        }
    }
}
