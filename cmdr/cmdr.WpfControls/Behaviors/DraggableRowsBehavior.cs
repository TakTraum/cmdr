using cmdr.WpfControls.Adorners;
using cmdr.WpfControls.Behaviors.SelectorAdapters;
using cmdr.WpfControls.Utils;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;

namespace cmdr.WpfControls.Behaviors
{
    /// <summary>
    /// ItemsSource must be ObservableCollection or at least IList!
    /// </summary>
    public class DraggableRowsBehavior
    {
        public class Data
        {
            public object SenderDataContext { get; set; }
            public bool IsMove { get; set; }
            public IList SelectedItems { get; set; }
            public int TargetIndex { get; set; }
        }


        private static readonly double MINIMUM_DRAG_DISTANCE_H = 2*SystemParameters.MinimumHorizontalDragDistance;
        private static readonly double MINIMUM_DRAG_DISTANCE_V = 2*SystemParameters.MinimumVerticalDragDistance;
        private static readonly double DRAG_SCROLL_TOLERANCE = 20;
        private static readonly double DRAG_SCROLL_OFFSET_DATAGRID = 2;
        private static readonly double DRAG_SCROLL_OFFSET_LISTBOX = 5;

        private static Point? _startPosition;
        private static IDataObject _dropData;
        private static ASelectorAdapter _senderAdapter;
        private static InsertAdorner INSERT_LINE;

        #region DependencyProperties

        #region IsEnabled

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(DraggableRowsBehavior), new PropertyMetadata(false, IsEnabledChanged));

        public static bool GetIsEnabled(Selector obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(Selector obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        #endregion

        #region SelectorAdapter

        private static readonly DependencyProperty SelectorAdapterProperty =
            DependencyProperty.RegisterAttached("SelectorAdapter", typeof(ASelectorAdapter), typeof(DraggableRowsBehavior), 
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));

        private static ASelectorAdapter GetSelectorAdapter(Selector d)
        {
            if (d == null)
                return null;

            return d.GetValue(SelectorAdapterProperty) as ASelectorAdapter;
        }

        private static void SetSelectorAdapter(Selector d, ASelectorAdapter value)
        {
            d.SetValue(SelectorAdapterProperty, value);
        }

        #endregion

        #region DropAction

        public static readonly DependencyProperty DropActionProperty =
            DependencyProperty.RegisterAttached("DropAction", typeof(ICommand), typeof(DraggableRowsBehavior),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));

        public static ICommand GetDropAction(Selector d)
        {
            return (ICommand)d.GetValue(DropActionProperty);
        }

        public static void SetDropAction(Selector d, ICommand value)
        {
            d.SetValue(DropActionProperty, value);
        }

        #endregion

        #endregion


        static DraggableRowsBehavior()
        {
            if (Application.Current != null && Application.Current.MainWindow != null)
                Application.Current.MainWindow.DragOver += OnWindowDragOver;
        }


        private static void IsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selector = (d as Selector);
            if (selector == null) 
                return;
      
            if ((bool)e.NewValue)
            {
                selector.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
                selector.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
                selector.PreviewMouseMove += OnMouseMove;
                selector.PreviewDragOver += OnGridPreviewDragOver;
                selector.PreviewDrop += OnGridPreviewDrop;

                if (selector is DataGrid)
                    SetSelectorAdapter(selector, new DataGridAdapter(selector));
                else if (selector is ListBox)
                    SetSelectorAdapter(selector, new ListBoxAdapter(selector));
            }
            else
            {
                selector.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
                selector.PreviewMouseLeftButtonUp -= OnMouseLeftButtonUp;
                selector.PreviewMouseMove -= OnMouseMove;
                selector.PreviewDragOver -= OnGridPreviewDragOver;
                selector.PreviewDrop -= OnGridPreviewDrop;

                if (selector is DataGrid)
                    SetSelectorAdapter(selector, null);
                else if (selector is ListBox)
                    SetSelectorAdapter(selector, null);
            }
        }


        #region Events

        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPosition = null;

            // don't mess with double clicks
            if (e.ClickCount > 1)
                return;

            var adapter = GetSelectorAdapter(sender as Selector);
            if (!adapter.IsDraggable(e.OriginalSource as DependencyObject))
                return;

            // don't select if a drag operation has started
            if (_dropData != null)
            {
                _dropData = null;
                e.Handled = true;
                return;
            }

            _startPosition = e.GetPosition(adapter.Selector);

            // this is essential for dragging multiple items, otherwise it's handled like clicking in a bunch of selected items.
            // since ctrl is used for copy operation, just make sure that the user can still alter the selection with shift.
            // ctrl for altering the selection is handled in ButtonUp event

            /*
             * pestrela 2019-12-27: this behavior worked for dragabbale rows, but introduced an annoying behaviour:
             * selecting a new range would take the starting point of the last range instead of the last single selection
             * example:
             *   click row1
             *   press shift
             *   click row "5"
             *   release shift
             *   click row "3"
             *   press shift
             *   click row "5"
             *   
             *   watch as row1 is still selected
             *
             * As such, disabling drag and drop for now
             * 
             */
            //if (!isShiftKeyPressed())
            //    e.Handled = true;
        }

        private static void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_startPosition == null)
                return;

            var adapter = GetSelectorAdapter(sender as Selector);

            var hoveredControl = e.OriginalSource as DependencyObject;
            if (!adapter.IsDraggable(hoveredControl))
                return;

            // if user is tuning the selection with modifiers
            if (!adapter.IsSelected(hoveredControl))
                return;

            if (isCtrlKeyPressed())  // alter selection manually
                adapter.Deselect(hoveredControl);
            else if (isShiftKeyPressed()) // alter selection by ignoring shift (see ButtonDown)
                return;
            else // if no modifier is pressed, discard current selection and just select the clicked item
                adapter.Select(hoveredControl, true);
        }

        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_startPosition == null || e.LeftButton != MouseButtonState.Pressed)
                return;

            var adapter = GetSelectorAdapter(sender as Selector);

            if (!isDragDistanceFulfilled(e.GetPosition(adapter.Selector)))
                return;

            // pick up data and start dragging
            var data = new Data
            {
                SenderDataContext = adapter.Selector.DataContext,
                SelectedItems = new ArrayList(adapter.GetSelectedItems()), // must be a copy
                TargetIndex = -1
            };
            _dropData = new DataObject(data);
            
            _startPosition = null;

            _senderAdapter = adapter;

            adapter.OnDragStarted();
            DragDrop.DoDragDrop(adapter.Selector, _dropData, DragDropEffects.Copy | DragDropEffects.Move);
            
            e.Handled = true;
        }

        private static void OnGridPreviewDragOver(object sender, DragEventArgs e)
        {
            if (_dropData == null)
                return;

            var adapter = GetSelectorAdapter(sender as Selector);

            var data = _dropData.GetData(typeof(Data)) as Data;
            bool sameSelectorTypes = _senderAdapter.GetType() == adapter.GetType();
            if (sameSelectorTypes)
            {
                var hoveredControl = e.OriginalSource as DependencyObject;

                // update drag visuals     
                var currentTargetItem = adapter.GetItem(hoveredControl);
                if (currentTargetItem != null)
                    showInsertLine(currentTargetItem, e.GetPosition(currentTargetItem).Y < currentTargetItem.ActualHeight / 4);
                else
                    hideInsertLine();

                if (adapter.IsValidDropTarget(hoveredControl))
                    e.Effects = (!isCtrlKeyPressed()) ? DragDropEffects.Move : DragDropEffects.Copy;
                else
                    e.Effects = DragDropEffects.None;
            }

            // scroll if necessary
            ScrollViewer sv = VisualHelpers.FindChild<ScrollViewer>(adapter.Selector);
            double verticalPos = e.GetPosition(adapter.Selector).Y;
            double scrollOffset = getDragScrollOffset(adapter);
            if (verticalPos < DRAG_SCROLL_TOLERANCE) // Top of visible list?
                sv.ScrollToVerticalOffset(sv.VerticalOffset - scrollOffset); //Scroll up.
            else if (verticalPos > adapter.Selector.ActualHeight - DRAG_SCROLL_TOLERANCE) //Bottom of visible list?
                sv.ScrollToVerticalOffset(sv.VerticalOffset + scrollOffset); //Scroll down.    

            e.Handled = sameSelectorTypes;
        }

        static void OnGridPreviewDrop(object sender, DragEventArgs e)
        {
            hideInsertLine();

            if (_dropData == null)
                return;

            // update data
            var data = _dropData.GetData(typeof(Data)) as Data;

            var adapter = GetSelectorAdapter(sender as Selector);
            var hoveredControl = e.OriginalSource as DependencyObject;

            var index = adapter.GetIndex(hoveredControl);
            if (index > -1)
            {
                data.TargetIndex = index;
                
                var item = adapter.GetItem(hoveredControl);
                if (e.GetPosition(item).Y >= item.ActualHeight / 4)
                    data.TargetIndex++;
            }

            data.IsMove = !isCtrlKeyPressed();

            ICommand iCommand = GetDropAction(adapter.Selector);
            if (iCommand != null && iCommand.CanExecute(e.Data))
                iCommand.Execute(e.Data);

            _dropData = null;
        }

        private static void OnWindowDragOver(object sender, DragEventArgs e)
        {
            // forbid invalid targets
            var adapter = GetSelectorAdapter(sender as Selector);
            var validTarget = adapter != null && adapter.IsValidDropTarget(e.OriginalSource as DependencyObject);
            if (!validTarget && _dropData != null)
            {
                e.Effects = DragDropEffects.None;
                hideInsertLine();
                e.Handled = true;
            }
        }

        #endregion

        
        private static void showInsertLine(UIElement visual, bool above)
        {
            hideInsertLine();

            var adornLayer = AdornerLayer.GetAdornerLayer(visual);
            if (adornLayer != null)
            {
                INSERT_LINE = new InsertAdorner(visual, above);
                adornLayer.Add(INSERT_LINE);
            }
        }

        private static void hideInsertLine()
        {
            if (INSERT_LINE == null || INSERT_LINE.AdornedElement == null)
                return;

            var adornLayer = AdornerLayer.GetAdornerLayer(INSERT_LINE.AdornedElement);
            if (adornLayer != null)
                adornLayer.Remove(INSERT_LINE);
        }

        private static bool isDragDistanceFulfilled(Point currentMousePos)
        {
            Vector diff = _startPosition.Value - currentMousePos;
            return (Math.Abs(diff.X) > MINIMUM_DRAG_DISTANCE_H ||
                Math.Abs(diff.Y) > MINIMUM_DRAG_DISTANCE_V);
        }

        private static bool isShiftKeyPressed()
        {
            return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        }

        private static bool isCtrlKeyPressed()
        {
            return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        }

        private static double getDragScrollOffset(ASelectorAdapter adapter)
        {
            if (adapter.Selector is DataGrid)
                return DRAG_SCROLL_OFFSET_DATAGRID;
            else if (adapter.Selector is ListBox)
                return DRAG_SCROLL_OFFSET_LISTBOX;
            return 1;
        }
    }
}
