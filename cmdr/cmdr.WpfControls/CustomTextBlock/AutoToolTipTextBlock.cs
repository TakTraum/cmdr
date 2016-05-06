using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace cmdr.WpfControls.CustomTextBlock
{
    public class AutoToolTipTextBlock : TextBlock
    {
        private readonly ToolTip _toolTip;

        public static readonly DependencyPropertyKey IsTextTrimmedKey =
            DependencyProperty.RegisterAttachedReadOnly("IsTextTrimmed", typeof(bool), typeof(AutoToolTipTextBlock), new PropertyMetadata(false));

        public static readonly DependencyProperty IsTextTrimmedProperty = IsTextTrimmedKey.DependencyProperty;


        public AutoToolTipTextBlock()
        {
            _toolTip = new ToolTip();
            ToolTip = _toolTip;
        }

        public Boolean GetIsTextTrimmed(TextBlock target)
        {
            return (Boolean)target.GetValue(IsTextTrimmedProperty);
        }

        private bool calculateIsTextTrimmed(TextBlock textBlock)
        {
            if (!textBlock.IsArrangeValid)
                return GetIsTextTrimmed(textBlock);

            Typeface typeface = new Typeface(
                textBlock.FontFamily,
                textBlock.FontStyle,
                textBlock.FontWeight,
                textBlock.FontStretch);

            // FormattedText is used to measure the whole width of the text held up by TextBlock container
            FormattedText formattedText = new FormattedText(
                textBlock.Text,
                CultureInfo.CurrentUICulture,
                textBlock.FlowDirection,
                typeface,
                textBlock.FontSize,
                textBlock.Foreground);

            formattedText.MaxTextWidth = textBlock.ActualWidth;

            return (formattedText.Height > textBlock.ActualHeight || formattedText.MinWidth > formattedText.MaxTextWidth);
        }

        private void setIsTextTrimmed(Boolean value)
        {
            SetValue(IsTextTrimmedKey, value);
        }

        private void updateIsTrimmed()
        {
            bool isTrimmed = false;
            if (TextTrimming != System.Windows.TextTrimming.None)
                isTrimmed = calculateIsTextTrimmed(this);

            setIsTextTrimmed(isTrimmed);
        }

        protected override void OnToolTipOpening(ToolTipEventArgs e)
        {
            base.OnToolTipOpening(e);
            updateIsTrimmed();
            if (GetIsTextTrimmed(this))
            {
                _toolTip.Content = Text;
                _toolTip.Visibility = System.Windows.Visibility.Visible;
            }
            else
                _toolTip.Visibility = Visibility.Collapsed;
        }
    }
}
