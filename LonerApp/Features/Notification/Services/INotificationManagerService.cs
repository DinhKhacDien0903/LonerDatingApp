namespace LonerApp.Features.Services;

public interface INotificationManagerService
{
    Task<GetNotificationResponse?> GetNotificationAsync(int currentPage, int pageSize, string UserId);
    Task<UpdateNotificationResponse> RemoveNotification(RemoveNotificationRequest request);
    Task<UpdateNotificationResponse> ReadNotification(ReadNotificationRequest request);
    Task<ClearNotificationResponse> ClearNotifications(string UserId);
}