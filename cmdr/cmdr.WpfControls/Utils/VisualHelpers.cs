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

        public static T FindChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }
    }
}
