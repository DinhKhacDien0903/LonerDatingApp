namespace LonerApp.Models
{
    public class GetMessagesResponse
    {
        public PaginatedResponse<MessageModel>? Messages { get; set; }
    }
    public partial class MessageModel : BaseModel
    {
        [ObservableProperty]
        private string? id;

        // [ObservableProperty]
        // private string? senderId;

        // [ObservableProperty]
        // private string? receiverId;

        [ObservableProperty]
        private string? content;

        [ObservableProperty]
        private DateTime sendTime;

        [ObservableProperty]
        private bool isCurrentUserSend;

        [ObservableProperty]
        private bool isRead;
        [ObservableProperty]
        private bool isImage;
    }

    public enum MessageType
    {
        Text,
        Image,
        Emoji
    }
}