using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace cmdr.WpfControls.DropDownButton
{
    public class DropDownButton : Button
    {
        Button button = null;
        ContextMenu menu = null;


        public System.Windows.Controls.Primitives.PlacementMode MenuPlacement
        {
            get { return (System.Windows.Controls.Primitives.PlacementMode)GetValue(MenuPlacementProperty); }
            set { SetValue(MenuPlacementProperty, value); }
        }
        public static readonly DependencyProperty MenuPlacementProperty =
            DependencyProperty.Register("MenuPlacement", typeof(System.Windows.Controls.Primitives.PlacementMode), typeof(DropDownButton), new PropertyMetadata(System.Windows.Controls.Primitives.PlacementMode.Center));

        public ObservableCollection<MenuItemViewModel> ItemsSource
        {
            get { return (ObservableCollection<MenuItemViewModel>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<MenuItemViewModel>), typeof(DropDownButton), new UIPropertyMetadata(new ObservableCollection<MenuItemViewModel>()));


        static DropDownButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton), new FrameworkPropertyMetadata(typeof(DropDownButton)));
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            button = GetTemplateChild("button") as Button;
            if (button != null)
                button.Click += onButtonClick;

            menu = GetTemplateChild("menu") as ContextMenu;
            if (menu != null)
                menu.Loaded += onMenuLoaded;
        }


        void onButtonClick(object sender, RoutedEventArgs e)
        {
            if (menu != null)
                menu.IsOpen = true;
        }

        void onMenuLoaded(object sender, RoutedEventArgs e)
        {
            if (button != null)
                button.ContextMenu.PlacementTarget = button;
        }
    }
}
