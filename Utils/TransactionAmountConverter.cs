using SmartSaving.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SmartSaving.Utils
{
    public  class TransactionAmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Transaction t)
            {
                return t.Type == TransactionType.Expense
                    ? $"-€{t.Amount:N2}"
                    : $"€{t.Amount:N2}";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
