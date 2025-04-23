namespace LonerApp.Features.Services;

public interface ISwipeService
{
    Task<GetProfilesResponse?> GetProfilesAsync(string endpoint, string UserId);
    Task<SwipeResponse?> SwipeAsyncAsync(SwipeRequest request);
}