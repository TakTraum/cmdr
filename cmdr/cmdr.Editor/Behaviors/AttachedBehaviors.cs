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
        public static readonly DependencyProperty DropBehaviourProperty =
            DependencyProperty.RegisterAttached("DropBehaviour", typeof(ICommand), typeof(AttachedBehaviors),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    OnDragDropBehaviourChanged));

        public static ICommand GetDropBehaviour(DependencyObject d)
        {
            return (ICommand)d.GetValue(DropBehaviourProperty);
        }

        public static void SetDropBehaviour(DependencyObject d, ICommand value)
        {
            d.SetValue(DropBehaviourProperty, value);
        }



        private static void OnDragDropBehaviourChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement g = d as UIElement;
            if (g != null)
            {
                g.Drop += (s, a) =>
                {
                    ICommand iCommand = GetDropBehaviour(d);
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
