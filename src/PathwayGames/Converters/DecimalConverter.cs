using System;
using System.Globalization;
using Xamarin.Forms;

namespace PathwayGames.Converters
{
    public class DecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is decimal)
                return value.ToString();
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string stringValue = value as string;
            if (stringValue != null && decimal.TryParse(stringValue.Replace(',', '.'), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal dec))
                return dec;
            return value;
        }
    }
}
