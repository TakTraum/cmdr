using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace cmdr.Editor
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private static SynchronizationContext _syncContext;
        public static ViewModels.ViewModel MainViewModel;


        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _syncContext = SynchronizationContext.Current;
            MainWindow mainWindow = new MainWindow();
            MainViewModel = new ViewModels.ViewModel(mainWindow.dockingManager);
            mainWindow.DataContext = MainViewModel;
            mainWindow.Show();
        }


        public static void SetStatus(string status)
        {
            _syncContext.Post(delegate
            {
                MainViewModel.StatusText = status;
            }, null);
        }

        public static void ResetStatus()
        {
            _syncContext.Post(delegate
            {
                MainViewModel.StatusText = null;
            }, null);
        }
    }
}
