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
        private readonly string LAYOUT_PATH = "Layout.xml";


        public TsiFileView()
        {
            InitializeComponent();

            Loaded += TsiFileView_Loaded;
            Unloaded += TsiFileView_Unloaded;
        }


        void TsiFileView_Unloaded(object sender, RoutedEventArgs e)
        {
            saveLayout();
        }

        void TsiFileView_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(LAYOUT_PATH))
                loadLayout();
            else
                initLayout();
        }

        private void initLayout()
        {
            initLayoutAnchorable(anchDevices);
            initLayoutAnchorable(anchDevEditor);
            initLayoutAnchorable(anchDetails);
        }

        private void initLayoutAnchorable(LayoutAnchorable la)
        {
            if (la.IsAutoHidden)
                la.ToggleAutoHide();

            var parent = la.Parent as LayoutAnchorablePane;
            if (parent != null)
                parent.DockWidth = new GridLength(la.AutoHideMinWidth);
        }

        private void saveLayout()
        {
            var serializer = new XmlLayoutSerializer(dockingManager);
            using (var stream = new StreamWriter(LAYOUT_PATH))
                serializer.Serialize(stream);
        }

        private void loadLayout()
        {
            var serializer = new XmlLayoutSerializer(dockingManager);
            using (var stream = new StreamReader(LAYOUT_PATH))
                serializer.Deserialize(stream);
        }
    }
}
