using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rustic.Converters
{
    public sealed class BooleanToVisibilityInvertedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ToBoolean(value) ? Visibility.Collapsed : Visibility.Visible;
        }

        private static bool ToBoolean(object value)
        {
            if (value is bool)
                return (bool)value;
            if (value is bool?)
                return (bool?)value == true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
                return (Visibility)value != Visibility.Visible;
            else
                return false;
        }
    }

}
