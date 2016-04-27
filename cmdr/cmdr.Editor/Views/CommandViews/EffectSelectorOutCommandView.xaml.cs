using cmdr.Editor.ViewModels;
using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Enums;
using System;
using System.Collections.Generic;

namespace cmdr.Editor.Views.CommandViews
{
    /// <summary>
    /// Interaktionslogik für OutCommandView.xaml
    /// </summary>
    public partial class EffectSelectorOutCommandView : CommandView
    {
        public sealed class EffectSelectorOutCommandWrapper<T> : ViewModelBase where T : struct, IConvertible
        {
            private readonly EffectSelectorOutCommand _command;

            public Dictionary<T, string> AllValues
            {
                get { return EnumOutCommand<T>.AllValues; }
            }

            public Effect Effect
            {
                get { return _command.ControllerRangeMin; }
                set { _command.ControllerRangeMin = _command.ControllerRangeMax = value; raisePropertyChanged("Effect"); }
            }

            public bool AllEffects
            {
                get { return _command.AllEffects; }
                set { _command.AllEffects = value; raisePropertyChanged("AllEffects"); raisePropertyChanged("Effect"); }
            }


            public EffectSelectorOutCommandWrapper(EffectSelectorOutCommand command)
            {
                _command = command;
            }
        }

        public EffectSelectorOutCommandView(ACommand command)
            : base(command, typeof(EffectSelectorOutCommandWrapper<>))
        {
            InitializeComponent();
        }
    }
}
