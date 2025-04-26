using System;

namespace LonerApp.Services;

public interface IChatService
{
    Task<GetProfilesResponse?> GetMatchedActiveUserAsync(string endpoint, string UserId);
    Task<GetBasicUserMessageResponse?> GetUserMessageAsync(string endpoint, string UserId);
    Task<GetMessagesResponse?> GetMessagesAsync(string endpoint, string queryParams);
}