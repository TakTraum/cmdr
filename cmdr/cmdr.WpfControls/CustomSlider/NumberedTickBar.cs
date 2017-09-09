using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace cmdr.WpfControls.CustomSlider
{
    internal class NumberedTickBar : TickBar
    {
        protected override void OnRender(DrawingContext dc)
        {
            Size size = new Size(base.ActualWidth, base.ActualHeight);
            int tickCount = (int)((this.Maximum - this.Minimum) / this.TickFrequency) + 1;
            if ((this.Maximum - this.Minimum) % this.TickFrequency == 0)
                tickCount -= 1;
            Double tickFrequencySize;
            // Calculate tick's setting
            tickFrequencySize = (size.Width * this.TickFrequency / (this.Maximum - this.Minimum));
            string text = "";
            FormattedText formattedText = null;
            double num = this.Maximum - this.Minimum;
            int i = 0;
            // Draw each tick text
            float val;
            for (i = 0; i <= tickCount; i++)
            {
                val = Convert.ToSingle(this.Minimum + this.TickFrequency * i);
                if (val % 1 == 0)
                    text = String.Format("{0:N0}", val);
                else
                    text = String.Format("{0:N1}", val);

                formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Verdana"), 8, Brushes.Black);
                dc.DrawText(formattedText, new Point((tickFrequencySize * i), 30));
            }
        }
    }
}
