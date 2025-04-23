namespace LonerApp.Models
{
    public class SwipeRequest
    {
        public string SwiperId { get; set; } = string.Empty;
        public string SwipedId { get; set; } = string.Empty;
        public bool Action { get; set; }
    }
}