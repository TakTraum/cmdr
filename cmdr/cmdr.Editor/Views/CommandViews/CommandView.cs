using cmdr.TsiLib.Commands;
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace cmdr.Editor.Views.CommandViews
{
    public class CommandView : UserControl
    {
        protected readonly INotifyPropertyChanged _wrapper;

        public event EventHandler ValueChanged;


        public CommandView(ACommand command, Type wrapperType)
        {
            try
            {
                _wrapper = buildWrapper(command, wrapperType);
                _wrapper.PropertyChanged += (s, e) => raiseValueChanged();
                DataContext = _wrapper;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot build Wrapper. Reason: " + ex.Message);
            }
        }

        private INotifyPropertyChanged buildWrapper(ACommand command, Type wrapperType)
        {
            Type t = command.GetType();
            while (!t.IsGenericType && t.BaseType != typeof(object))
                t = t.BaseType;
            if (t.IsGenericType)
            {
                Type constructedType = wrapperType.MakeGenericType(t.GenericTypeArguments);
                return (INotifyPropertyChanged)Activator.CreateInstance(constructedType, new object[] { command });
            }
            return null;
        }

        private void raiseValueChanged()
        {
            if (ValueChanged != null)
                ValueChanged(this, new EventArgs());
        }
    }
}
