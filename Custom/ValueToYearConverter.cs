using System;
using System.Collections.Generic;
using System.Globalization;
using SofyTrender.Pages;

namespace SofyTrender.Custom
{
    public class ValueToYearConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var years = Utils.ConvertFromValues((double)values[0], (double)values[1]);
            return $" {years.minYear} - {years.maxYear}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            string[] years = ((string)value).Split('-');
            var values = Utils.ConvertFromYears(int.Parse(years[0]), int.Parse(years[1]));
            return (new[] { values.min, values.max}).Cast<object>().ToArray() ;
        }
    }
}
