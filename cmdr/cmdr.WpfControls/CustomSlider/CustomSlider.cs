using System.Windows;
using System.Windows.Controls;

namespace cmdr.WpfControls.CustomSlider
{
    public class CustomSlider : Slider
    {
        static CustomSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomSlider), new FrameworkPropertyMetadata(typeof(CustomSlider)));
        }
    }
}
