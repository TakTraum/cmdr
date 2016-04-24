using cmdr.Editor.ViewModels;
using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Ranges;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;

namespace cmdr.Editor.Views.CommandViews
{
    /// <summary>
    /// Interaktionslogik für IntOutCommandView.xaml
    /// </summary>
    public partial class IntOutCommandView : CommandView
    {
        public sealed class IntOutCommandWrapper<T> : ViewModelBase where T : IntRange, new()
        {
            private readonly IntOutCommand<T> _command;

            public int MinValue { get { return _command.MinValue; } }
            public int MaxValue { get { return _command.MaxValue; } }

            public int ControllerRangeMin
            {
                get { return _command.ControllerRangeMin; }
                set { _command.ControllerRangeMin = value; raisePropertyChanged("ControllerRangeMin"); }
            }

            public int ControllerRangeMax
            {
                get { return _command.ControllerRangeMax; }
                set { _command.ControllerRangeMax = value; raisePropertyChanged("ControllerRangeMax"); }
            }

            public int TickFrequency { get { return (int)Math.Floor((double)(MaxValue - MinValue) / 10); } }

            public IntOutCommandWrapper(IntOutCommand<T> command)
            {
                _command = command;
            }
        }

        public IntOutCommandView(ACommand command)
            : base(command, typeof(IntOutCommandWrapper<>))
        {
            InitializeComponent();
        }


        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var tb = sender as TextBox;
            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                float val;
                if (float.TryParse(tb.Text, System.Globalization.NumberStyles.Integer, CultureInfo.CurrentCulture, out val))
                {
                    if (e.Key == Key.Up)
                        tb.Text = (val + 1).ToString();
                    else
                        tb.Text = (val - 1).ToString();
                }
            }
        }
    }
}
