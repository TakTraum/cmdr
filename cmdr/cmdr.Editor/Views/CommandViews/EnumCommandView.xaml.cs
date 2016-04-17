using cmdr.Editor.ViewModels;
using cmdr.TsiLib.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace cmdr.Editor.Views.CommandViews
{
    /// <summary>
    /// Interaktionslogik für EnumCommandView.xaml
    /// </summary>
    public partial class EnumCommandView : CommandView
    {
        public sealed class EnumInCommandWrapper<T> : ViewModelBase where T : struct, IConvertible
        {
            private readonly EnumInCommand<T> _command;

            public Dictionary<T, string> AllValues
            {
                get { return EnumInCommand<T>.AllValues; }
            }

            public T Value
            {
                get { return _command.Value; }
                set { _command.Value = value; raisePropertyChanged("Value"); }
            }

            public EnumInCommandWrapper(EnumInCommand<T> command)
            {
                _command = command;
            }
        }

        public EnumCommandView(ACommand command)
            : base(command, typeof(EnumInCommandWrapper<>))
        {
            InitializeComponent();
        }
    }
}
