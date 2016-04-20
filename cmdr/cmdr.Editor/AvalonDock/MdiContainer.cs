using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace cmdr.Editor.AvalonDock
{
    public sealed class MdiContainer<T, U, V> where T: MdiChild<U, V> where U: UserControl
    {
        private DockingManager _dockingManager;
        private ObservableCollection<LayoutContent> _internalMdiChildren { get { return _dockingManager.Layout.Descendents().OfType<LayoutDocumentPane>().First().Children; } }

        public event OnMdiChildClosingDelegate OnClosing;
        public event OnMdiChildClosedDelegate OnClosed;
        public event OnMdiChildSelectedDelegate OnSelected;

        private Dictionary<string, T> _mdiChildren;
        public ReadOnlyDictionary<string, T> MdiChildren
        {
            get { return new ReadOnlyDictionary<string,T>(_mdiChildren); }
        }

        public T SelectedMdiChild
        {
            get
            {
                var selectedId = _internalMdiChildren.FirstOrDefault(c => c.IsSelected);
                if (selectedId != null)
                    return _mdiChildren[selectedId.ContentId];
                return null;
            }
        }


        public MdiContainer(DockingManager dockingManager)
        {
            _dockingManager = dockingManager;
            _mdiChildren = new Dictionary<string, T>();
        }


        public void SelectMdiChild(string id)
        {
            var selectedId = _internalMdiChildren.FirstOrDefault(c => c.ContentId == id);
            if (selectedId != null)
                selectedId.IsSelected = true;
        }

        public void RemoveMdiChild(string id, bool silent = false)
        {
            var child = _internalMdiChildren.FirstOrDefault(c => c.ContentId == id);
            if (child != null)
            {
                if (!silent)
                {
                    var e = new CancelEventArgs();
                    if (OnClosing != null)
                        OnClosing(child.ContentId, e);

                    if (e.Cancel)
                        return;

                    if (OnClosed != null)
                        OnClosed(child.ContentId);
                }

                try
                {
                    child.Close();
                }
                catch { }
            }
        }

        public void AddMdiChild(T child)
        {
            var mdiChild = new LayoutDocument
            {
                Content = child.View,
                ContentId = child.Id
            };

            Binding b = new Binding
            {
                Path = new System.Windows.PropertyPath("Title"),
                Source = child,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            BindingOperations.SetBinding(mdiChild, LayoutDocument.TitleProperty, b);

            mdiChild.Closing += (s, e) =>
                {
                    var c = s as LayoutDocument;
                    
                    if (OnClosing != null)
                        OnClosing(c.ContentId, e);
                };

            mdiChild.Closed += (s, e) =>
                {
                    var c = s as LayoutDocument;
                    
                    if (OnClosed != null)
                        OnClosed(c.ContentId);

                    _mdiChildren.Remove(c.ContentId);
                };

            mdiChild.IsSelectedChanged += (s, e) =>
                {
                    var c = s as LayoutDocument;
                    if (c.IsSelected && OnSelected != null)
                        OnSelected(c.ContentId);
                };

            _mdiChildren.Add(child.Id, child);
            _internalMdiChildren.Add(mdiChild);

            mdiChild.IsSelected = true;
        }
    }

    public delegate void OnMdiChildClosingDelegate(string id, CancelEventArgs e);
    public delegate void OnMdiChildClosedDelegate(string id);
    public delegate void OnMdiChildSelectedDelegate(string id);
}
