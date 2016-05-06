using cmdr.WpfControls.CustomTextBlock;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace cmdr.WpfControls.CustomDataGrid
{
    public class CustomDataGridTextColumn : DataGridTextColumn
    {
        protected override System.Windows.FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            AutoToolTipTextBlock textBlock = new AutoToolTipTextBlock();
            textBlock.TextTrimming = System.Windows.TextTrimming.CharacterEllipsis;

            syncProperties(textBlock);
            applyStyle(/* isEditing = */ false, /* defaultToElementStyle = */ false, textBlock);
            applyBinding(textBlock, TextBlock.TextProperty);

            return textBlock;
        }


        private void syncProperties(FrameworkElement e)
        {
            syncColumnProperty(this, e, TextElement.FontFamilyProperty, FontFamilyProperty);
            syncColumnProperty(this, e, TextElement.FontSizeProperty, FontSizeProperty);
            syncColumnProperty(this, e, TextElement.FontStyleProperty, FontStyleProperty);
            syncColumnProperty(this, e, TextElement.FontWeightProperty, FontWeightProperty);
            syncColumnProperty(this, e, TextElement.ForegroundProperty, ForegroundProperty);
        }

        private void syncColumnProperty(DependencyObject column, DependencyObject content, DependencyProperty contentProperty, DependencyProperty columnProperty)
        {
            if (isDefaultValue(column, columnProperty))
                content.ClearValue(contentProperty);
            else
                content.SetValue(contentProperty, column.GetValue(columnProperty));
        }

        private bool isDefaultValue(DependencyObject d, DependencyProperty dp)
        {
            return DependencyPropertyHelper.GetValueSource(d, dp).BaseValueSource == BaseValueSource.Default;
        }

        private void applyBinding(DependencyObject target, DependencyProperty property)
        {
            var binding = Binding;
            if (binding == null)
                BindingOperations.ClearBinding(target, property);
            else
                BindingOperations.SetBinding(target, property, binding);
        }

        private void applyStyle(bool isEditing, bool defaultToElementStyle, FrameworkElement element)
        {
            Style style = this.pickStyle(isEditing, defaultToElementStyle);
            if (style != null)
                element.Style = style;
        }

        private Style pickStyle(bool isEditing, bool defaultToElementStyle)
        {
            Style elementStyle = isEditing ? this.EditingElementStyle : this.ElementStyle;
            if ((isEditing && defaultToElementStyle) && (elementStyle == null))
                elementStyle = this.ElementStyle;
            return elementStyle;
        }
    }
}
