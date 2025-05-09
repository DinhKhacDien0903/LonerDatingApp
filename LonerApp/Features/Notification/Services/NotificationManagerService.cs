namespace LonerApp.Features.Services;

public class NotificationManagerService : INotificationManagerService
{
    private readonly IApiService _apiService;
    public NotificationManagerService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<ClearNotificationResponse> ClearNotifications(string UserId)
    {
        try
        {
            string queryParams = $"?UserId={UserId}";
            await _apiService.DeleteAsync(EnvironmentsExtensions.ENDPOINT_CLEAR_NOTIFICATIONS, queryParams);
            return new ClearNotificationResponse
            {
                Message = "Notifications cleared successfully",
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error during swipe operation: {ex.Message}", ex);
            return new ClearNotificationResponse
            {
                Message = "Notifications cleared failed!",
                IsSuccess = false
            };
        }
    }

    public async Task<GetNotificationResponse?> GetNotificationAsync(int currentPage, int pageSize, string UserId)
    {
        try
        {
            string queryParams = $"?PaginationRequest.PageNumber={currentPage}&PaginationRequest.PageSize={pageSize}&PaginationRequest.UserId={UserId}";
            var response = await _apiService.GetAsync<GetNotificationResponse>(EnvironmentsExtensions.ENDPOINT_GET_NOTIFICATIONS, queryParams);
            return response;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error during swipe operation: {ex.Message}", ex);
            return null;
        }
    }

    public async Task<UpdateNotificationResponse> ReadNotification(ReadNotificationRequest request)
    {
        try
        {
            var response = await _apiService.PostAsync<UpdateNotificationResponse>(EnvironmentsExtensions.ENDPOINT_READ_NOTIFICATION, request);
            return response;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error during swipe operation: {ex.Message}", ex);
            return new UpdateNotificationResponse
            {
                Message = "Notifications cleared failed!",
                IsSuccess = false
            };
        }
    }

    public async Task<UpdateNotificationResponse> RemoveNotification(RemoveNotificationRequest request)
    {
        try
        {
            var response = await _apiService.PostAsync<UpdateNotificationResponse>(EnvironmentsExtensions.ENDPOINT_REMOVE_NOTIFICATION, request);
            return response;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error during swipe operation: {ex.Message}", ex);
            return new UpdateNotificationResponse
            {
                Message = "Notifications remove failed!",
                IsSuccess = false
            };
        }
    }
}