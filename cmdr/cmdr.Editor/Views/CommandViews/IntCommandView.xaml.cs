using cmdr.Editor.ViewModels;
using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Ranges;
using System;
using System.Globalization;
using System.Windows.Input;

namespace cmdr.Editor.Views.CommandViews
{
    /// <summary>
    /// Interaktionslogik für IntCommandView.xaml
    /// </summary>
    public partial class IntCommandView : CommandView
    {
        public sealed class IntInCommandWrapper<T> : ViewModelBase where T : IntRange, new()
        {
            private readonly IntInCommand<T> _command;

            public int MinValue { get { return _command.MinValue; } }
            public int MaxValue { get { return _command.MaxValue; } }

            public int Value
            {
                get { return _command.Value; }
                set { _command.Value = value; raisePropertyChanged("Value"); }
            }

            public int TickFrequency { get { return (int)Math.Floor((double)(MaxValue - MinValue) / 10); } }

            public IntInCommandWrapper(IntInCommand<T> command)
            {
                _command = command;
            }
        }


        public IntCommandView(ACommand command)
            : base(command, typeof(IntInCommandWrapper<>))
        {
            InitializeComponent();
        }


        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                float val;
                if (float.TryParse(tbValue.Text, System.Globalization.NumberStyles.Integer, CultureInfo.CurrentCulture, out val))
                {
                    if (e.Key == Key.Up)
                        tbValue.Text = (val + 1).ToString();
                    else
                        tbValue.Text = (val - 1).ToString();
                }
            }
        }
    }
}
