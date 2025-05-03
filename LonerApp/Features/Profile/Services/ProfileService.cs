using System.Net;

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

    public async Task<UpdateUserInforResponse?> UpdateUserInforAsync(UpdateUserInforRequest request)
    {
        try
        {
            var response = await _apiService.PostAsync<UpdateUserInforResponse>(EnvironmentsExtensions.ENDPOINT_UPDATE_USER_PROFILE, request);
            return response;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error during swipe operation: {ex.Message}", ex);
            return null;
        }
    }
}