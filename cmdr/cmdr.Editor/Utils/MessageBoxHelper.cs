using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace cmdr.Editor.Utils
{
    static class MessageBoxHelper
    {
        public static void ShowInfo(string message)
        {
            MessageBox.Show(App.Current.MainWindow, message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowWarning(string message)
        {
            MessageBox.Show(App.Current.MainWindow, message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void ShowError(string message)
        {
            MessageBox.Show(App.Current.MainWindow, message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static bool ShowQuestion(string message)
        {
            return MessageBox.Show(App.Current.MainWindow, message, "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public static bool? ShowCancellableQuestion(string message)
        {
            var result = MessageBox.Show(App.Current.MainWindow, message, "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    return true;
                case MessageBoxResult.No:
                    return false;
                default:
                    return null;
            }
        }
    }
}
