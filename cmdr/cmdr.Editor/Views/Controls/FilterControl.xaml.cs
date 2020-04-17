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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace cmdr.Editor.Views.Controls
{
    /// <summary>
    /// Interaktionslogik für SearchControl.xaml
    /// </summary>
    public partial class FilterControl : UserControl
    {
        public FilterControl()
        {
            InitializeComponent();
        }

        private void SearchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            if (popup.IsOpen)
            {
                popup.IsOpen = false;
                return;
            }

            SearchViewModel svm = DataContext as SearchViewModel;
            if (svm != null)
            {
                if (!svm.IsFound)
                    return;
                
                if (!svm.Continue())
                    popup.IsOpen = true;
            }
        }
    }
}
