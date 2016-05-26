using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cmdr.WpfControls.CustomListBox
{
    public class CustomListBox : ListBox
    {
        private static readonly double DRAG_SCROLL_TOLERANCE = 20;
        private static readonly double DRAG_SCROLL_OFFSET = 2;


        static CustomListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomListBox), new FrameworkPropertyMetadata(typeof(CustomListBox)));
        }


        protected override void OnPreviewDragOver(DragEventArgs e)
        {
            base.OnPreviewDragOver(e);

            double verticalPos = e.GetPosition(this).Y;

            ScrollViewer sv =  Template.FindName("PART_SV", this) as ScrollViewer;
            if (sv == null)
                return;

            if (verticalPos < DRAG_SCROLL_TOLERANCE) // Top of visible list?
                sv.ScrollToVerticalOffset(sv.VerticalOffset - DRAG_SCROLL_OFFSET); //Scroll up.
            else if (verticalPos > this.ActualHeight - DRAG_SCROLL_TOLERANCE) //Bottom of visible list?
                sv.ScrollToVerticalOffset(sv.VerticalOffset + DRAG_SCROLL_OFFSET); //Scroll down.    
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (e.AddedItems != null && e.AddedItems.Count > 0)
                ScrollIntoView(e.AddedItems[e.AddedItems.Count - 1]);
        }
    }
}
