
namespace LonerApp.Features.Services;

public class NotificationManagerService : INotificationManagerService
{
    private readonly IApiService _apiService;
    public NotificationManagerService(IApiService apiService)
    {
        _apiService = apiService;
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
}