namespace LonerApp.Helpers.Converters
{
    public class ImageContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is byte[] imageBytes)
            {
                return ImageSource.FromStream(() => new MemoryStream(imageBytes));
            }
            else if (value is string imagePath)
            {
                return ImageSource.FromFile(imagePath);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}