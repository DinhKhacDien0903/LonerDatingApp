namespace LonerApp.Features.Services;

public class SwipeService : ISwipeService
{
    private readonly IApiService _apiService;
    public SwipeService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<PaginatedResponse<UserProfileResponse>?> GetProfilesAsync(string endpoint, string UserId)
    {
        try
        {
            var response = await _apiService.GetAsync<PaginatedResponse<UserProfileResponse>>(endpoint, UserId);
            return response;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error during swipe operation: {ex.Message}", ex);
            return null;
        }
    }
}