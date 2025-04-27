namespace LonerApp.Features.Chat.Models
{
    public class SendMessageRequestDto
    {
        public string? MessageId { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string? ReceiverId { get; set; }
        public string MatchId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsImage { get; set; }
        public DateTime? SendTime { get; set; } = DateTime.UtcNow;
    }
}