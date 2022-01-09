using System;
using System.Text;
using System.Windows.Data;

namespace ZiPatch_Explorer.Converters;

[ValueConversion(typeof(byte[]), typeof(string))]
public class ByteArrayConverter : IValueConverter
{
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return BitConverter.ToString((byte[])value).Replace("-", "").ToUpper();
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    #endregion
}