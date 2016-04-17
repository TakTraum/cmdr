using cmdr.Editor.ViewModels;
using cmdr.TsiLib.Commands;
using System;
using System.Collections.Generic;

namespace cmdr.Editor.Views.CommandViews
{
    /// <summary>
    /// Interaktionslogik für OutCommandView.xaml
    /// </summary>
    public partial class EnumOutCommandView : CommandView
    {
        public sealed class EnumOutCommandWrapper<T> : ViewModelBase where T : struct, IConvertible
        {
            private readonly EnumOutCommand<T> _command;

            public Dictionary<T, string> AllValues
            {
                get { return EnumOutCommand<T>.AllValues; }
            }

            public T ControllerRangeMin
            {
                get { return _command.ControllerRangeMin; }
                set { _command.ControllerRangeMin = value; raisePropertyChanged("ControllerRangeMin"); }
            }

            public T ControllerRangeMax
            {
                get { return _command.ControllerRangeMax; }
                set { _command.ControllerRangeMax = value; raisePropertyChanged("ControllerRangeMax"); }
            }


            public EnumOutCommandWrapper(EnumOutCommand<T> command)
            {
                _command = command;
            }
        }

        public EnumOutCommandView(ACommand command)
            : base(command, typeof(EnumOutCommandWrapper<>))
        {
            InitializeComponent();
        }
    }
}
