using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MauiApp1
{
    public class NullableStringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isNullOrEmpty = string.IsNullOrWhiteSpace(value as string);
            if (parameter != null && parameter.ToString() == "!")
            {
                return isNullOrEmpty;
            }
            return !isNullOrEmpty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
