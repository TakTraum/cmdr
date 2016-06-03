using System.Windows;
using System.Windows.Controls;

namespace cmdr.WpfControls.CustomListBox
{
    public class CustomListBox : ListBox
    {
        static CustomListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomListBox), new FrameworkPropertyMetadata(typeof(CustomListBox)));
        }
    }
}
