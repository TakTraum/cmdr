using System;
using System.Threading;
using System.Windows.Input;

namespace cmdr.Editor.Utils
{
    public class CommandHandler : ICommand
    {
        private readonly Action _action;
        private readonly Func<bool> _canExecute;
        private readonly SynchronizationContext syncCtx;


        public CommandHandler(Action action, Func<bool> canExecuteFunction=null)
        {
            _action = action;
            _canExecute = canExecuteFunction ?? (() => true);
            syncCtx = SynchronizationContext.Current;
        }


        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _action();
        }

        public void UpdateCanExecuteState()
        {
            syncCtx.Post(delegate { CommandManager.InvalidateRequerySuggested(); }, null);
        }
    }


    public class CommandHandler<T> : ICommand
    {
        private readonly Action<T> _action;
        private readonly Func<bool> _canExecute;
        private readonly SynchronizationContext syncCtx;


        public CommandHandler(Action<T> action, Func<bool> canExecuteFunction=null)
        {
            _action = action;
            _canExecute = canExecuteFunction ?? (() => true);
            syncCtx = SynchronizationContext.Current;
        }


        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _action((T)parameter);
        }

        public void UpdateCanExecuteState()
        {
            syncCtx.Post(delegate { CommandManager.InvalidateRequerySuggested(); }, null);
        }
    }
}
