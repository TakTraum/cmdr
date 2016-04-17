using cmdr.Editor.ViewModels;
using cmdr.TsiLib.Commands;
using cmdr.TsiLib.Ranges;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;

namespace cmdr.Editor.Views.CommandViews
{
    /// <summary>
    /// Interaktionslogik für FloatOutCommandView.xaml
    /// </summary>
    public partial class FloatOutCommandView : CommandView
    {
        public sealed class FloatOutCommandWrapper<T> : ViewModelBase where T : FloatRange, new()
        {
            private readonly FloatOutCommand<T> _command;

            public float MinValue { get { return _command.MinValue; } }
            public float MaxValue { get { return _command.MaxValue; } }

            public float ControllerRangeMin
            {
                get { return _command.ControllerRangeMin; }
                set { _command.ControllerRangeMin = value; raisePropertyChanged("ControllerRangeMin"); }
            }

            public float ControllerRangeMax
            {
                get { return _command.ControllerRangeMax; }
                set { _command.ControllerRangeMax = value; raisePropertyChanged("ControllerRangeMax"); }
            }


            public FloatOutCommandWrapper(FloatOutCommand<T> command)
            {
                _command = command;
            }
        }

        public FloatOutCommandView(ACommand command)
            : base(command, typeof(FloatOutCommandWrapper<>))
        {
            InitializeComponent();
        }


        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var tb = sender as TextBox;
            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                float val;
                if (float.TryParse(tb.Text, System.Globalization.NumberStyles.Float, CultureInfo.CurrentCulture, out val))
                {
                    if (e.Key == Key.Up)
                        tb.Text = (val + 0.001f).ToString();
                    else
                        tb.Text = (val - 0.001f).ToString();
                }
            }
        }
    }
}
