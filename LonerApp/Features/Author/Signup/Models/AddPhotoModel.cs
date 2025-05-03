namespace LonerApp.Models
{
    public partial class AddPhotoModel : BaseModel
    {
        [ObservableProperty]
        string _imagePath = "blank_image.png";
        [ObservableProperty]
        string _iconPath;
        public bool IsDefaultImage { get; private set; } = true;

        partial void OnImagePathChanged(string oldValue, string newValue)
        {
            if (newValue is string file && file == "blank_image.png")
            {
                IsDefaultImage = true;
                IconPath = "\uf417";
            }
            else
            {
                IsDefaultImage = false;
                IconPath = "\uf6fe";
            }
        }
    }
}