using cmdr.WpfControls.CustomDataGrid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace cmdr.WpfControls.Test
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ViewModel;
            var selectionCopy = new List<RowItemViewModel>(vm.SelectedThings);
            vm.SelectedThings.Clear();

            foreach (var item in selectionCopy)
            {
                var copy = (item.Item as cmdr.WpfControls.Test.ViewModel.Thing).Copy();
                copy.Number *= 2;
                vm.Things.Add(new CustomDataGrid.RowItemViewModel(copy));
            }
        }
    }
}
