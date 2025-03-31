namespace LonerApp.Models
{
    public partial class MessageModel : BaseModel
    {
        [ObservableProperty]
        private string id;

        [ObservableProperty]
        private string? senderId;

        [ObservableProperty]
        private string? receiverId;

        [ObservableProperty]
        private string? message;

        [ObservableProperty]
        private MessageType type;

        [ObservableProperty]
        private DateTime timestamp;

        [ObservableProperty]
        private bool isMine;

        [ObservableProperty]
        private bool isRead;
    }

    public enum MessageType
    {
        Text,
        Image,
        Emoji
    }
}