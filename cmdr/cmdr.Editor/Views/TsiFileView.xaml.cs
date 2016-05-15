using cmdr.Editor.AvalonDock;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace cmdr.Editor.Views
{
    /// <summary>
    /// Interaktionslogik für TsiFileView.xaml
    /// </summary>
    public partial class TsiFileView : UserControl
    {
        private readonly LayoutManager _layoutManager;


        public TsiFileView()
        {
            InitializeComponent();
            _layoutManager = new LayoutManager(dockingManager);

            Loaded += TsiFileView_Loaded;
            Unloaded += TsiFileView_Unloaded;
        }


        void TsiFileView_Unloaded(object sender, RoutedEventArgs e)
        {
            _layoutManager.SaveLayout();
        }

        void TsiFileView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_layoutManager.DefaultLayoutAvailable)
                _layoutManager.LoadLayout();
            else
                initLayout();
        }

        private void initLayout()
        {
            initLayout(anchDevices);
            initLayout(anchDevEditor);
            initLayout(anchDetails);
        }

        private void initLayout(LayoutAnchorable la)
        {
            if (la.IsAutoHidden)
                la.ToggleAutoHide();

            var parent = la.Parent as LayoutAnchorablePane;
            if (parent != null)
                parent.DockWidth = new GridLength(la.AutoHideMinWidth);
        }
    }
}
