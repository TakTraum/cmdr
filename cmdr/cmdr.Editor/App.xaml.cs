using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
//using WindowPlacement;

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
            String appTheme = "Light";
            //String appTheme = "Dark";
            this.Resources.MergedDictionaries[0].Source =
                new Uri($"/Styles/Themes/{appTheme}.xaml", UriKind.Relative);

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

        /*
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.MainWindowPlacement = this.GetPlacement();
            Settings.Default.Save();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.SetPlacement(Settings.Default.MainWindowPlacement);
        }*/

    }
}
