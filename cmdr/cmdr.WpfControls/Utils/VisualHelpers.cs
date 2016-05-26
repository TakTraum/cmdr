using System.Windows;
using System.Windows.Media;

namespace cmdr.WpfControls.Utils
{
    public static class VisualHelpers
    {
        public static T FindAncestor<T>(object current) where T : DependencyObject
        {
            if (current == null || !(current is Visual))
                return null;
            do
            {
                if (current is T)
                    return (T)current;
                current = VisualTreeHelper.GetParent(current as DependencyObject);
            }
            while (current != null);

            return null;
        }
    }
}
