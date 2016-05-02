using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace cmdr.WpfControls.Converters
{
    [ValueConversion(typeof(Uri), typeof(Image))]
    public class UriToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            Image image = null;
            try
            {
                Uri uri = value as Uri;
                BitmapSource bitmapSource = new BitmapImage(uri);
                image = new Image { Source = bitmapSource };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
