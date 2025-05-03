using System.Globalization;

namespace LonerApp.Helpers.Converters
{
    public class BooleanToGenderConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool gender)
                return gender ? "Nam" : "Nữ";
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}