namespace LonerApp.Models
{
    public partial class UserChatModel : BaseModel
    {
        [ObservableProperty]
        private int _userId; 

        [ObservableProperty]
        private string _userName = string.Empty;

        [ObservableProperty]
        private string _profilePicture = string.Empty;

        [ObservableProperty]
        private string _lastMessage = string.Empty;

        [ObservableProperty]
        private bool _isUnRead;
    }
}
