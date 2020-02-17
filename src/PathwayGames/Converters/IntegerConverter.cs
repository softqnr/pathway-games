using System;
using System.Globalization;
using Xamarin.Forms;

namespace PathwayGames.Converters
{
    public class IntegerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value as string) && value is int)
                return value.ToString();
            return 0.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringValue = value as string;
            if (stringValue != null && int.TryParse(stringValue.Replace(',', '.'), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out int intValue))
                return intValue;
            return value;
        }
    }
}
