using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace cmdr.Editor.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class PluralizationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");

            int val = (int)value;
            string paramStr = parameter.ToString();
            if (val == 0 || val > 1)
                paramStr = PluralizationService.CreateService(culture).Pluralize(paramStr);
            return String.Format("{0} " + paramStr, val);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
