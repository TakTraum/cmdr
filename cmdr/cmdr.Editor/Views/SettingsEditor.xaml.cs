using SettingControlLibrary.SettingControls;
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

namespace cmdr.Editor.Views
{
    /// <summary>
    /// Interaktionslogik für SettingsEditor.xaml
    /// </summary>
    public partial class SettingsEditor : UserControl
    {  
        public IEnumerable<BaseSettingControl> SettingControls { get; private set; }

        public SettingsEditor(IEnumerable<BaseSettingControl> settingControls)
        {
            InitializeComponent();
            SettingControls = settingControls;
            DataContext = this;
        }

        public new void Focus()
        {
            base.Focus();
            if (SettingControls.Any())
                SettingControls.First().Focus();
        }
    }
}
