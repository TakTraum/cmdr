using cmdr.Editor.ViewModels;
using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Ranges;
using System;
using System.Globalization;
using System.Windows.Input;

namespace cmdr.Editor.Views.CommandViews
{
    /// <summary>
    /// Interaktionslogik für CenteredFloatCommandView.xaml
    /// </summary>
    public partial class FloatCommandView : CommandView
    {
        public sealed class FloatInCommandWrapper<T> : ViewModelBase where T : FloatRange, new()
        {
            private readonly FloatInCommand<T> _command;

            public float MinValue { get { return _command.MinValue; } }
            public float MaxValue { get { return _command.MaxValue; } }

            public float Value
            {
                get { return _command.Value; }
                set { _command.Value = value; raisePropertyChanged("Value"); }
            }

            public float TickFrequency { get { return (MaxValue - MinValue) / 10f; } }

            public FloatInCommandWrapper(FloatInCommand<T> command)
            {
                _command = command;
            }
        }


        public FloatCommandView(ACommand command)
            : base(command, typeof(FloatInCommandWrapper<>))
        {
            InitializeComponent();
        }


        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                float val;
                if (float.TryParse(tbValue.Text, System.Globalization.NumberStyles.Float, CultureInfo.CurrentCulture, out val))
                {
                    if (e.Key == Key.Up)
                        tbValue.Text = (val + 0.001f).ToString();
                    else
                        tbValue.Text = (val - 0.001f).ToString();
                }
            }
        }
    }
}
