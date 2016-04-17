using System.Windows;
using cmdr.Editor.ViewModels;

namespace cmdr.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = new ViewModel(dockingManager);
            DataContext = _vm;
        }
    }
}
