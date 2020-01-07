using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace cmdr.Editor.ViewModels.Comment
{
    public class CommentEditorViewModel : ViewModelBase
    {
        private readonly IEnumerable<MappingViewModel> _mappings;

        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                raisePropertyChanged("Comment");

                foreach (var mvm in _mappings)
                    mvm.Comment = _comment;
            }
        }


        public CommentEditorViewModel(IEnumerable<MappingViewModel> mappings)
        {
            _mappings = mappings;

            var common = _mappings.Select(m => m.Comment).Distinct();
            if (common.Count() == 1)
                _comment = common.Single();
            else
                _comment = String.Empty;

            // workaround for inline editing in mapping list
            var first = _mappings.FirstOrDefault();
            if (first != null)
                first.PropertyChanged += onCommentChangedInline;
        }


        // todo: add a pop-up list of selected comments
        private void onCommentChangedInline(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Comment":
                    _comment = (sender as MappingViewModel).Comment;
                    raisePropertyChanged("Comment");
                    break;
                default:
                    break;
            }
        }
    }
}
