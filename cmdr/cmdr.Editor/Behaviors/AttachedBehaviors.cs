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
                    OnDropBehaviourChanged));

        public static ICommand GetDropBehaviour(DependencyObject d)
        {
            return (ICommand)d.GetValue(DropBehaviourProperty);
        }

        public static void SetDropBehaviour(DependencyObject d, ICommand value)
        {
            d.SetValue(DropBehaviourProperty, value);
        }


        public static readonly DependencyProperty DragOverBehaviourProperty =
            DependencyProperty.RegisterAttached("DragOverBehaviour", typeof(ICommand), typeof(AttachedBehaviors),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    OnDragOverBehaviourChanged));

        public static ICommand GetDragOverBehaviour(DependencyObject d)
        {
            return (ICommand)d.GetValue(DragOverBehaviourProperty);
        }

        public static void SetDragOverBehaviour(DependencyObject d, ICommand value)
        {
            d.SetValue(DragOverBehaviourProperty, value);
        }


        private static void OnDropBehaviourChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement g = d as UIElement;
            if (g != null)
            {
                g.Drop += (s, a) =>
                {
                    ICommand iCommand = GetDropBehaviour(d);
                    if (iCommand != null)
                        if (iCommand.CanExecute(a.Data))
                            iCommand.Execute(a.Data);
                };
            }
            else
                throw new ApplicationException("Non UIElement");
        }

        private static void OnDragOverBehaviourChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement g = d as UIElement;
            if (g != null)
            {
                g.DragOver += (s, a) =>
                {
                    ICommand iCommand = GetDragOverBehaviour(d);
                    if (iCommand != null)
                        if (iCommand.CanExecute(a))
                            iCommand.Execute(a);
                };
            }
            else
                throw new ApplicationException("Non UIElement");
        }
    }
}
