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
        private string? content;

        [ObservableProperty]
        private MessageType type;

        [ObservableProperty]
        private DateTime timestamp;

        [ObservableProperty]
        private bool isMine;

        [ObservableProperty]
        private bool isRead;

        [ObservableProperty]
        private byte[]? imageData;
    }

    public enum MessageType
    {
        Text,
        Image,
        Emoji
    }
}