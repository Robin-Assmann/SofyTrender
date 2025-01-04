using System.Globalization;
using SofyTrender.Pages;

namespace SofyTrender.Custom
{
    public class ValueToYearConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var (minYear, maxYear) = Utils.ConvertFromValues((double)values[0], (double)values[1]);
            return $" {minYear} - {maxYear}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            string[] years = ((string)value).Split('-');
            var (min, max) = Utils.ConvertFromYears(int.Parse(years[0]), int.Parse(years[1]));
            return (new[] { min, max }).Cast<object>().ToArray();
        }
    }
}
