using System;
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace cmdr.WpfControls.CustomDataGrid
{
    public class DraggableRowsBehavior
    {
        public class Data
        {
            public object SenderDataContext { get; set; }
            public bool IsMove { get; set; }
            public IList SelectedItems { get; set; }
            public int TargetIndex { get; set; }
        }



        private static readonly double MINIMUM_DRAG_DISTANCE_H = SystemParameters.MinimumHorizontalDragDistance;
        private static readonly double MINIMUM_DRAG_DISTANCE_V = SystemParameters.MinimumVerticalDragDistance;
        private static readonly double DRAG_SCROLL_TOLERANCE = 20;
        private static readonly double DRAG_SCROLL_OFFSET = 2;


        private static Point? _startPosition;
        private static IDataObject _dropData;
        private static InsertAdorner INSERT_LINE;

        #region DependencyProperties

        // ItemsSource must be ObservableCollection or at least IList    
        public static readonly DependencyProperty EnableRowsMoveProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(DraggableRowsBehavior), new PropertyMetadata(false, EnableRowsMoveChanged));


        public static bool GetIsEnabled(DataGrid obj)
        {
            return (bool)obj.GetValue(EnableRowsMoveProperty);
        }

        public static void SetIsEnabled(DataGrid obj, bool value)
        {
            obj.SetValue(EnableRowsMoveProperty, value);
        }

        public static readonly DependencyProperty DropActionProperty =
            DependencyProperty.RegisterAttached("DropAction", typeof(ICommand), typeof(DraggableRowsBehavior),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None));

        public static ICommand GetDropAction(DependencyObject d)
        {
            return (ICommand)d.GetValue(DropActionProperty);
        }

        public static void SetDropAction(DependencyObject d, ICommand value)
        {
            d.SetValue(DropActionProperty, value);
        }

        #endregion


        static DraggableRowsBehavior()
        {
            if (Application.Current != null && Application.Current.MainWindow != null)
                Application.Current.MainWindow.DragOver += OnWindowDragOver;
        }


        private static void EnableRowsMoveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = (d as DataGrid);
            if (grid == null) 
                return;
      
            if ((bool)e.NewValue)
            {
                grid.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
                grid.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
                grid.PreviewMouseMove += OnMouseMove;
                grid.PreviewDragOver += OnGridPreviewDragOver;
                grid.PreviewDrop += OnGridPreviewDrop;
            }
            else
            {
                grid.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
                grid.PreviewMouseLeftButtonUp -= OnMouseLeftButtonUp;
                grid.PreviewMouseMove -= OnMouseMove;
                grid.PreviewDragOver -= OnGridPreviewDragOver;
                grid.PreviewDrop -= OnGridPreviewDrop;
            }
        }


        #region Events

        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var row = findAncestor<DataGridRow>(e.OriginalSource);
            if (row == null || row.IsEditing)
                return;

            // don't select if a drag operation has started
            if (_dropData != null)
            {
                _dropData = null;
                e.Handled = true;
                return;
            }


            // only allow drag for already selected rows
            DataGrid grid = sender as DataGrid;
            var selectedItems = grid.SelectedItems;
            if (selectedItems.Contains(row.Item))
            {
                _startPosition = e.GetPosition(grid);

                // this is essential for dragging multiple items, otherwise it's handled like clicking in a bunch of selected items.
                // since ctrl is used for copy operation, just make sure that the user can still alter the selection with shift.
                // ctrl for altering the selection is handled in ButtonUp event
                if (!isShiftKeyPressed())
                    e.Handled = true;
            }
        }

        private static void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            var pos = e.GetPosition(grid);
            bool wasClickOnSelectedItem = !isDragDistanceFulfilled(pos);
            _startPosition = null;

            if (wasClickOnSelectedItem)
            {
                var row = findAncestor<DataGridRow>(e.OriginalSource);
                if (row != null && !row.IsEditing)
                {
                    var itemUnderMouse = row.Item;

                    // if user is tuning the selection with modifiers
                    if (grid.SelectedItems.Contains(itemUnderMouse))
                    {
                        if (isCtrlKeyPressed())  // alter selection manually
                            grid.SelectedItems.Remove(itemUnderMouse);
                        else if (isShiftKeyPressed()) // alter selection by ignoring shift (see ButtonDown)
                            return;
                        else
                        {
                            // if no modifier is pressed, discard current selection and just select the clicked item
                            grid.SelectedItems.Clear();
                            grid.SelectedItem = itemUnderMouse;
                        }
                    }
                }
            }
        }

        private static void OnMouseMove(object sender, MouseEventArgs e)
        {            
            if (_startPosition == null)
                return;

            DataGrid dataGrid = sender as DataGrid;
            if (e.LeftButton == MouseButtonState.Pressed && isDragDistanceFulfilled(e.GetPosition(dataGrid)))
            {
                if (dataGrid != null)
                {
                    // pick up data and start dragging
                    var data = new Data
                    {
                        SenderDataContext = dataGrid.DataContext,
                        SelectedItems = dataGrid.SelectedItems.Cast<RowItemViewModel>().ToList(),
                        TargetIndex = -1
                    };

                    _dropData = new DataObject(data);

                    // reset sorting
                    resetSorting(dataGrid);

                    DragDrop.DoDragDrop(dataGrid, _dropData, DragDropEffects.Copy | DragDropEffects.Move);

                    e.Handled = true;
                    _startPosition = null;
                }
            }
        }

        private static void OnGridPreviewDragOver(object sender, DragEventArgs e)
        {
            if (_dropData == null)
                return;

            // update drag visuals
            var hoveredControl = e.OriginalSource as DependencyObject;
            
            var row = findAncestor<DataGridRow>(hoveredControl);
            if (row != null)
                showInsertLine(row, e.GetPosition(row).Y < row.ActualHeight/4);
            else
                hideInsertLine();

            var header = findAncestor<DataGridColumnHeader>(hoveredControl);
            if (header != null)
                e.Effects = DragDropEffects.None;
            else
                e.Effects = (!isCtrlKeyPressed()) ? DragDropEffects.Move : DragDropEffects.Copy;

            DataGrid dataGrid = sender as DataGrid;
            double verticalPos = e.GetPosition(dataGrid).Y;
            
            ScrollViewer sv = findChild<ScrollViewer>(dataGrid);
            if (verticalPos < DRAG_SCROLL_TOLERANCE) // Top of visible list?
                sv.ScrollToVerticalOffset(sv.VerticalOffset - DRAG_SCROLL_OFFSET); //Scroll up.
            else if (verticalPos > dataGrid.ActualHeight - DRAG_SCROLL_TOLERANCE) //Bottom of visible list?
                sv.ScrollToVerticalOffset(sv.VerticalOffset + DRAG_SCROLL_OFFSET); //Scroll down.    

            e.Handled = true;
        }

        static void OnGridPreviewDrop(object sender, DragEventArgs e)
        {
            hideInsertLine();

            if (_dropData != null)
            {
                // update data
                var data = _dropData.GetData(typeof(Data)) as Data;

                var hoveredControl = e.OriginalSource as DependencyObject;
                var grid = findAncestor<DataGrid>(hoveredControl);
                if (grid != null)
                {
                    var row = findAncestor<DataGridRow>(hoveredControl);
                    if (row != null)
                    {
                        data.TargetIndex = grid.Items.IndexOf(row.Item);
                        if (e.GetPosition(row).Y >= row.ActualHeight / 4)
                            data.TargetIndex++;
                    }
                }

                data.IsMove = !isCtrlKeyPressed();

                ICommand iCommand = GetDropAction(sender as DataGrid);
                if (iCommand != null && iCommand.CanExecute(e.Data))
                    iCommand.Execute(e.Data);

                _dropData = null;
            }
        }

        private static void OnWindowDragOver(object sender, DragEventArgs e)
        {
            // forbid other targets than DataGrids
            var grid = findAncestor<DataGrid>(e.OriginalSource as DependencyObject);
            if (grid == null && _dropData != null)
            {
                e.Effects = DragDropEffects.None;
                hideInsertLine();
                e.Handled = true;
            }
        }

        #endregion


        private static void resetSorting(DataGrid grid)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(grid.ItemsSource);
            if (view != null && view.SortDescriptions != null)
            {
                view.SortDescriptions.Clear();
                foreach (var column in grid.Columns)
                    column.SortDirection = null;
            }
        }

        private static void showInsertLine(DataGridRow row, bool above)
        {
            hideInsertLine();

            var adornLayer = AdornerLayer.GetAdornerLayer(row);
            if (row != null)
            {
                INSERT_LINE = new InsertAdorner(row, above);
                adornLayer.Add(INSERT_LINE);
            }
        }

        private static void hideInsertLine()
        {
            if (INSERT_LINE == null)
                return;

            DataGridRow row = INSERT_LINE.AdornedElement as DataGridRow;
            if (row == null)
                return;

            var adornLayer = AdornerLayer.GetAdornerLayer(row);
            if (adornLayer != null)
                adornLayer.Remove(INSERT_LINE);
        }

        private static bool isDragDistanceFulfilled(Point currentMousePos)
        {
            if (_startPosition == null)
                return true;

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

        private static T findAncestor<T>(object current) where T : DependencyObject
        {
            if (current == null || !(current is Visual))
                return null;
            do
            {
                if (current is T)
                    return (T)current;
                current = VisualTreeHelper.GetParent(current as DependencyObject);
            }
            while (current != null);

            return null;
        }

        private static T findChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = findChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }
    }
}
