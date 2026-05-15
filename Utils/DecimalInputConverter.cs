using System;
using System.Globalization;
using System.Windows.Data;


namespace SmartSaving.Utils
{
    public  class DecimalInputConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal d && d == 0) return string.Empty;
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                if (string.IsNullOrWhiteSpace(s)) return 0m;

                s = s.Replace(",", ".").Trim();

                // Strictly reject anything that isn't purely digits with optional single decimal point
                if (!System.Text.RegularExpressions.Regex.IsMatch(s, @"^[0-9]+(\.[0-9]+)?$"))
                    return 0m;

                if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                    return result;
            }
            return 0m;
        }


    }
}
