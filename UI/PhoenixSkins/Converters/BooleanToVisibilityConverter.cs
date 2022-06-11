using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PhoenixSkins.Converters
{
    /// <summary>
    /// bool转Visibility，false：Visible，true：Collapsed
    /// </summary>
    public class ContraryBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Visible;
            }

            return !GetBool(value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        private static bool GetBool(object value)
        {
            if (value is bool)
            {
                return (bool)value;
            }

            return false;
        }
    }

    /// <summary>
    /// bool转Visibility，false：Hidden，true：Visible
    /// </summary>
    public class BooleanToHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Visible;
            }

            return GetBool(value) ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        private static bool GetBool(object value)
        {
            if (value is bool)
            {
                return (bool)value;
            }

            return false;
        }
    }
}
