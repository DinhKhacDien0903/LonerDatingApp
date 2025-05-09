namespace LonerApp.Models;

public partial class NotificationResponse : BaseModel
{
        public string? Id { get; set; } = string.Empty;
        public string? Messeage { get; set; }
        public string ReceiverId { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        [ObservableProperty]
        private bool _isRead;
        public int Type { get; set; } = 0; // 0: like, 1:match, 2: message
        public string RelatedId { get; set; } = string.Empty; // UserId or MessageId or MatchId or SwipeId
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string? NotificationImage { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
}

public class UpdateNotificationRequest : NotificationResponse
{
}