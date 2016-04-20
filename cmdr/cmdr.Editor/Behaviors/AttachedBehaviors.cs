using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace cmdr.Editor.Behaviors
{
    public static class AttachedBehaviors
    {
        public static readonly DependencyProperty DragDropBehaviourProperty =
            DependencyProperty.RegisterAttached("DragDropBehaviour", typeof(ICommand), typeof(AttachedBehaviors),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    OnDragDropBehaviourChanged));

        public static ICommand GetDragDropBehaviour(DependencyObject d)
        {
            return (ICommand)d.GetValue(DragDropBehaviourProperty);
        }

        public static void SetDragDropBehaviour(DependencyObject d, ICommand value)
        {
            d.SetValue(DragDropBehaviourProperty, value);
        }

        private static void OnDragDropBehaviourChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement g = d as UIElement;
            if (g != null)
            {
                g.Drop += (s, a) =>
                {
                    ICommand iCommand = GetDragDropBehaviour(d);
                    if (iCommand != null)
                    {
                        if (iCommand.CanExecute(a.Data))
                        {
                            iCommand.Execute(a.Data);
                        }
                    }
                };
            }
            else
            {
                throw new ApplicationException("Non UIElement");
            }
        }
    }
}
