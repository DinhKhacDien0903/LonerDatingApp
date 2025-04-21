using LonerApp.Apis;

namespace LonerApp.Features.Services;

public class ProfileService : IProfileService
{
    private readonly IApiService _apiService;
    public ProfileService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<GetProfileDetailResponse?> GetProfileDetailAsync(string endpoint, string UserId)
    {
        try
        {
            var response = await _apiService.GetAsync<GetProfileDetailResponse>(endpoint, UserId);
            return response;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error during swipe operation: {ex.Message}", ex);
            return null;
        }
    }
}