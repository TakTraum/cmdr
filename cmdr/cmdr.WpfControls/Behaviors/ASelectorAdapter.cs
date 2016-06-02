using System.Collections;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace cmdr.WpfControls.Behaviors
{
    abstract class ASelectorAdapter
    {
        private Selector _selector;
        public Selector Selector
        {
            get { return _selector; }
        }


        public ASelectorAdapter(Selector selector)
        {
            _selector = selector;
        }


        public abstract bool IsDraggable(DependencyObject control);
        public abstract bool IsValidDropTarget(DependencyObject control);
        public abstract FrameworkElement GetItem(DependencyObject control);
        public abstract bool IsSelected(DependencyObject control);
        public abstract void Select(DependencyObject control, bool exclusive);
        public abstract void Deselect(DependencyObject control);
        public abstract IList GetSelectedItems();
        public abstract int GetIndex(DependencyObject control);
        
        public virtual void OnDragStarted()
        {

        }
    }
}
