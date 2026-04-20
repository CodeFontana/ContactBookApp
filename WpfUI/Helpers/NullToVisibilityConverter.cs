using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfUI.Helpers;

public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isNull = value is null;
        bool invert = parameter is string s && string.Equals(s, "Invert", StringComparison.OrdinalIgnoreCase);

        bool show = invert ? isNull : !isNull;
        return show ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
