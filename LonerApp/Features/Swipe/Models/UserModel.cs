using System.Collections.ObjectModel;

namespace LonerApp.Models
{
    public partial class UserModel : BaseModel
    {
        [ObservableProperty]
        string _name = string.Empty;
        [ObservableProperty]
        string _status = string.Empty; 
        [ObservableProperty]
        string _university = string.Empty;
        [ObservableProperty]
        string _distance = "50 km"; 
        [ObservableProperty]
        string _work = "Developer";
        [ObservableProperty]
        string _address = string.Empty;
        [ObservableProperty]
        string _gender = string.Empty;       
        [ObservableProperty]
        int _age;        
        [ObservableProperty]
        string _about;
        [ObservableProperty]
        ImageSource _image = string.Empty;        
        [ObservableProperty]
        ObservableCollection<AddPhotoModel> _listImage;
        [ObservableProperty]
        ObservableCollection<string> _interests = new ObservableCollection<string> { "Interest 1", "Interest 2", "Interest 3", "Interest 3", "Interest 3", "Interest 3" };
        // [ObservableProperty]
        // ObservableCollection<string> _images = new ObservableCollection<string>();
    }
}