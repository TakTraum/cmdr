using ChangeTracking;
using cmdr.Editor.Metadata;
using cmdr.TsiLib.Conditions;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace cmdr.Editor.ViewModels.Metadata
{
    public class DeviceMetadataViewModel : AReversible
    {
        private readonly DeviceMetadata _metadata;


        public ObservableCollection<Tuple<int, MappingMetadata>> MappingMetadata { get; private set; }

        public ObservableCollection<Tuple<ACondition, string>> ConditionDescriptions { get; private set; }


        public DeviceMetadataViewModel(DeviceMetadata metadata)
        {
            _metadata = metadata;

            MappingMetadata = new ObservableCollection<Tuple<int, MappingMetadata>>(_metadata.MappingMetadata.Select(m => new Tuple<int, MappingMetadata>(m.Key, m.Value)));
            MappingMetadata.CollectionChanged += onMappingMetadataChanged;

            //ConditionDescriptions = new ObservableCollection<Tuple<ACondition, string>>(_metadata.ConditionDescriptions.Select(c => new Tuple<ACondition, string>(c.Key, c.Value)));
        }


        private void onMappingMetadataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsChanged = true;
        }


        protected override void Accept()
        {
            _metadata.MappingMetadata = MappingMetadata.ToDictionary(m => m.Item1, m => m.Item2);

        }

        protected override void Revert()
        {
            MappingMetadata.CollectionChanged -= onMappingMetadataChanged;
            MappingMetadata = new ObservableCollection<Tuple<int, MappingMetadata>>(_metadata.MappingMetadata.Select(m => new Tuple<int, MappingMetadata>(m.Key, m.Value)));
            MappingMetadata.CollectionChanged += onMappingMetadataChanged;
        }
    }
}
