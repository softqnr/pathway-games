using System;
using System.Globalization;
using Xamarin.Forms;

namespace PathwayGames.Converters
{
    public class CheckedChangedEventArgsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var eventArgs = value as CheckedChangedEventArgs;
            if (eventArgs == null)
                throw new ArgumentException("Expected CheckedChangedEventArgs as value", "value");

            return eventArgs.Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}