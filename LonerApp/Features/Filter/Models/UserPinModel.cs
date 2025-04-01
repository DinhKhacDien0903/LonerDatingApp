using Microsoft.Maui.Controls.Maps;

namespace LonerApp.Models
{
    public partial class UserPinModel : BaseModel
    {
        [ObservableProperty]
        string _label = string.Empty; 
        [ObservableProperty]
        string _address = string.Empty;
        [ObservableProperty]
        PinType _type;
        [ObservableProperty]
        Location? _location;
    }
}