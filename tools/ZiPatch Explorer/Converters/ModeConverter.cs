using System;
using System.Globalization;
using System.Windows.Data;

namespace ZiPatch_Explorer.Converters;

[ValueConversion(typeof(int), typeof(char))]
public class ModeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return System.Convert.ToChar(value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}