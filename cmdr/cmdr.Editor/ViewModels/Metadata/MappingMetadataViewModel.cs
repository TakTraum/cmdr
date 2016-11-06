using ChangeTracking;
using cmdr.Editor.Metadata;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace cmdr.Editor.ViewModels.Metadata
{
    public class MappingMetadataViewModel : AReversible
    {
        private readonly MappingMetadata _metadata;


        public bool IsLocked
        {
            get { return _metadata.IsLocked; }
            set { _metadata.IsLocked = value; raisePropertyChanged("IsLocked"); IsChanged = true; }
        }

        public ObservableCollection<string> Tags { get; private set; }


        public MappingMetadataViewModel(MappingMetadata metadata)
        {
            _metadata = metadata;

            Tags = new ObservableCollection<string>(_metadata.Tags);
            Tags.CollectionChanged += onTagsChanged;
        }


        private void onTagsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsChanged = true;
        }


        protected override void Accept()
        {
            _metadata.Tags = Tags.ToList();
        }

        protected override void Revert()
        {
            Tags.CollectionChanged -= onTagsChanged;
            Tags = new ObservableCollection<string>(_metadata.Tags);
            Tags.CollectionChanged += onTagsChanged;
        }
    }
}
