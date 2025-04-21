
namespace LonerApp.Features.Services;

public interface ISwipeService
{
    Task<PaginatedResponse<UserProfileResponse>?> GetProfilesAsync(string endpoint, string UserId);
}