using cmdr.Editor.ViewModels;
using cmdr.TsiLib.EventArgs;
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
    /// Interaktionslogik für EffectIdentificationWindow.xaml
    /// </summary>
    public partial class EffectIdentificationWindow : Window
    {
        private readonly EffectIdentificationRequest _request;


        public EffectIdentificationWindow(EffectIdentificationRequest request)
        {
            _request = request;
            InitializeComponent();
            Loaded += EffectIdentificationWindow_Loaded;
        }

        void EffectIdentificationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = new EffectIdentificationViewModel(_request);
            this.DataContext = vm;

            if (vm.CloseAction == null)
                vm.CloseAction = new Action(Close);

            if (vm.Window == null)
                vm.Window = this;
        }
    }
}
