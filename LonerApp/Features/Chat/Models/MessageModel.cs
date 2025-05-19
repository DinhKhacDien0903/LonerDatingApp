namespace LonerApp.Models
{
    public class GetMessagesResponse
    {
        public PaginatedResponse<MessageModel>? Messages { get; set; }
    }

    public class SendMessageResponse
    {
        public string MessageId { get; set; } = string.Empty;
    }

    public class SendMessageRequest
    {
        public MessageModel MessageRequest { get; set; } = new ();
    }
    public partial class MessageModel : BaseModel
    {
        [ObservableProperty]
        private string? id;

        [ObservableProperty]
        private string? senderId;

        [ObservableProperty]
        private string? receiverId;

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
        public string? MatchId { get; set; }
        public bool IsMessageOfChatBot { get; set; }
    }

    public enum MessageType
    {
        Text,
        Image,
        Emoji
    }
}