namespace LonerApp.Models;

public class GetNotificationResponse
{
    public PaginatedResponse<NotificationResponse>? Notifications { get; set; }
}

public class UpdateNotificationResponse
{
    public string? Message { get; set; }
    public bool IsSuccess { get; set; }
}

public class ClearNotificationResponse : UpdateNotificationResponse
{
}

public class GetNotificationsRequest
{
    public NotificationResponse Notification { get; set; } = new();
}

public class RemoveNotificationRequest
{
    public NotificationResponse Notification { get; set; } = new();
}

public class ReadNotificationRequest
{
    public NotificationResponse Notification { get; set; } = new();
}