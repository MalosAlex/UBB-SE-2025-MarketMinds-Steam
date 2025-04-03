using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace SteamProfile.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
            {
                // If parameter is "inverse", invert the boolean value
                if (parameter?.ToString()?.ToLower() == "inverse")
                {
                    boolValue = !boolValue;
                }
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility visibility)
            {
                bool result = visibility == Visibility.Visible;
                // If parameter is "inverse", invert the boolean value
                if (parameter?.ToString()?.ToLower() == "inverse")
                {
                    result = !result;
                }
                return result;
            }
            return false;
        }
    }
}