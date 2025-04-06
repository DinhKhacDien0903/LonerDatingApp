namespace LonerApp.Models
{
    public partial class AddPhotoModel : BaseModel
    {
        //[ObservableProperty]
        //ImageSource _imagePath = ImageSource.FromFile("blank_image.png"); 
        [ObservableProperty]
        ImageSource _imagePath = ImageSource.FromFile("blank_image.png");
        [ObservableProperty]
        string _iconPath = "\uf417";
        public bool IsDefaultImage { get; private set; } = true;

        partial void OnImagePathChanged(ImageSource oldValue, ImageSource newValue)
        {
            if (newValue is FileImageSource file && file.File == "blank_image.png")
            {
                IsDefaultImage = true;
            }
            else
            {
                IsDefaultImage = false;
            }
        }
    }
}