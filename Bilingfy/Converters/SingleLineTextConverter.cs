using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Bilingfy.Converters;

public class SingleLineTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            return str.Replace("\r\n", " ")
                      .Replace("\n", " ")
                      .Replace("\r", " ");
        }
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

