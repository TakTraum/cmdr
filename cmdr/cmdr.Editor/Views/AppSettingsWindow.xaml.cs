using cmdr.Editor.ViewModels;
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
using System.Windows.Shapes;

namespace cmdr.Editor.Views
{
    /// <summary>
    /// Interaktionslogik für AppSettingsWindow.xaml
    /// </summary>
    public partial class AppSettingsWindow : Window
    {
        public AppSettingsWindow()
        {
            InitializeComponent();
            Loaded += AppSettingsWindow_Loaded;
        }

        void AppSettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = new AppSettingsViewModel();
            this.DataContext = vm;

            if (vm.CloseAction == null)
                vm.CloseAction = new Action(Close);

            if (vm.Window == null)
                vm.Window = this;
        }
    }
}
