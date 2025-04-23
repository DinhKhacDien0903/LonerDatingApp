using System.Net;

namespace LonerApp.Features.Services;

public class SwipeService : ISwipeService
{
    private readonly IApiService _apiService;
    public SwipeService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<GetProfilesResponse?> GetProfilesAsync(string endpoint, string UserId)
    {
        try
        {
            var response = await _apiService.GetAsync<GetProfilesResponse>(endpoint, UserId);
            return response;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error during swipe operation: {ex.Message}", ex);
            return null;
        }
    }

    public async Task<SwipeResponse?> SwipeAsyncAsync(SwipeRequest request)
    {
        try
        {
            var response = await _apiService.PostAsync<SwipeResponse>(EnvironmentsExtensions.ENDPOINT_SWIPE_USER, request);
            return response;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error during swipe operation: {ex.Message}", ex);
            return null;
        }
    }
}