using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace cmdr.WpfControls.Behaviors
{
    // code inspired by user SpudCZ's post at http://stackoverflow.com/a/3247575

    public static class WindowBehavior
    {
        private static readonly Type OwnerType = typeof(WindowBehavior);

        #region CanUserClose (attached property)

        public static readonly DependencyProperty CanUserCloseProperty =
            DependencyProperty.RegisterAttached(
                "CanUserClose",
                typeof(bool),
                OwnerType,
                new FrameworkPropertyMetadata(true, new PropertyChangedCallback(CanUserCloseChangedCallback)));

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static bool GetCanUserClose(Window obj)
        {
            return (bool)obj.GetValue(CanUserCloseProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static void SetCanUserClose(Window obj, bool value)
        {
            obj.SetValue(CanUserCloseProperty, value);
        }

        private static void CanUserCloseChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window == null) return;

            var canUserClose = (bool)e.NewValue;
            if (!canUserClose && !GetIsHiddenCloseButton(window))
            {
                if (!window.IsLoaded)
                    window.Loaded += HideCloseButtonWhenLoadedDelegate;
                else
                    HideCloseButton(window);

                window.PreviewKeyDown += OnPreviewKeyDown;

                SetIsHiddenCloseButton(window, true);
            }
            else if (canUserClose && GetIsHiddenCloseButton(window))
            {
                if (!window.IsLoaded)
                    window.Loaded += ShowCloseButtonWhenLoadedDelegate;
                else
                    ShowCloseButton(window);

                window.PreviewKeyDown -= OnPreviewKeyDown;

                SetIsHiddenCloseButton(window, false);
            }
        }

        private static void OnPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // ignore Esc and Alt+F4
            if (e.Key == System.Windows.Input.Key.Escape)
                e.Handled = true;
            else if (e.Key == System.Windows.Input.Key.System && e.SystemKey == System.Windows.Input.Key.F4)
                e.Handled = true;
        }

        #region Win32 imports

        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        #endregion

        private static readonly RoutedEventHandler HideCloseButtonWhenLoadedDelegate = (sender, args) =>
        {
            if (sender is Window == false) return;
            var w = (Window)sender;
            HideCloseButton(w);
            w.Loaded -= HideCloseButtonWhenLoadedDelegate;
        };

        private static readonly RoutedEventHandler ShowCloseButtonWhenLoadedDelegate = (sender, args) =>
        {
            if (sender is Window == false) return;
            var w = (Window)sender;
            ShowCloseButton(w);
            w.Loaded -= ShowCloseButtonWhenLoadedDelegate;
        };

        private static void HideCloseButton(Window w)
        {
            var hwnd = new WindowInteropHelper(w).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private static void ShowCloseButton(Window w)
        {
            var hwnd = new WindowInteropHelper(w).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) | WS_SYSMENU);
        }

        #endregion

        #region IsHiddenCloseButton (readonly attached property)

        private static readonly DependencyPropertyKey IsHiddenCloseButtonKey =
            DependencyProperty.RegisterAttachedReadOnly(
                "IsHiddenCloseButton",
                typeof(bool),
                OwnerType,
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsHiddenCloseButtonProperty =
            IsHiddenCloseButtonKey.DependencyProperty;

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static bool GetIsHiddenCloseButton(Window obj)
        {
            return (bool)obj.GetValue(IsHiddenCloseButtonProperty);
        }

        private static void SetIsHiddenCloseButton(Window obj, bool value)
        {
            obj.SetValue(IsHiddenCloseButtonKey, value);
        }

        #endregion
    }
}
