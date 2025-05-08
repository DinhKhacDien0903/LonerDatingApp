namespace LonerApp.Models;

public class GetNotificationResponse
{
    public PaginatedResponse<NotificationResponse>? Notifications { get; set; }
}