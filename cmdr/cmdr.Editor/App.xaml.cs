using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace cmdr.Editor
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public static ViewModels.ViewModel MainViewModel;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            MainViewModel = new ViewModels.ViewModel(mainWindow.dockingManager);
            mainWindow.DataContext = MainViewModel;
            this.MainWindow = mainWindow;

            mainWindow.Show();
        }


        public static void SetStatus(string status)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                MainViewModel.StatusText = status;
            }), DispatcherPriority.Background);
        }

        public static void ResetStatus()
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                MainViewModel.StatusText = null;
            }), DispatcherPriority.Background);
        }
    }
}
