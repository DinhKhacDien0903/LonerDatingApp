namespace LonerApp.Models
{
    public partial class UserChatModel : BaseModel
    {
        [ObservableProperty]
        private string _userId = string.Empty;
        [ObservableProperty]
        private string _matchId = string.Empty;

        [ObservableProperty]
        private string _userName = string.Empty;

        [ObservableProperty]
        private string _avatarUrl = string.Empty;

        [ObservableProperty]
        private string _lastMessage = string.Empty;

        [ObservableProperty]
        private bool _isCurrentUserSend;

        [ObservableProperty]
        private DateTime? sendTime;
    }

    public class GetBasicUserMessageResponse
    {
        public PaginatedResponse<UserChatModel>? UserMessages { get; set; }
    }
}