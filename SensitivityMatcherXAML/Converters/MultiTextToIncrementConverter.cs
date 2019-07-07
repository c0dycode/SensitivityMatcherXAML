using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SensitivityMatcherXAML.Converters
{
    public class MultiTextToIncrementConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0].ToString() != string.Empty && values[1].ToString() != string.Empty)
            {
                double one;
                double two;
                var bOne = double.TryParse(values[0].ToString(), out one);
                var bTwo = double.TryParse(values[1].ToString(), out two);
                if(bOne && bTwo)
                    return Math.Round(one * two, 6).ToString();
            }
            return "0";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
