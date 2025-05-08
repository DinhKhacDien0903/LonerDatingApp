namespace LonerApp.Features.Services;

public interface INotificationManagerService
{
    Task<GetNotificationResponse?> GetNotificationAsync(int currentPage, int pageSize, string UserId);
}