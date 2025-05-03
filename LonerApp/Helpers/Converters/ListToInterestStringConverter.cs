using System.Globalization;
using System.Text;

namespace LonerApp.Helpers.Converters
{
    public class ListToInterestStringConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not IEnumerable<string> interests)
                return "";
            StringBuilder result = new();
            foreach(var item in interests)
            {
                result.Append($"{item}, ");
            }

            return result.ToString();
        }
        
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
