namespace LonerApp.Features.Chat.Services;

public class ChatService : IChatService
{
    private readonly IApiService _apiService;
    public ChatService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<PromptResponse?> GenerateByGeminiAsync(PromptRequest request)
    {
        try
        {
            var response = await _apiService.PostAsync<PromptResponse>(EnvironmentsExtensions.ENDPOINT_GENERATE_GEMINI, request);
            return response;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error during swipe operation: {ex.Message}", ex);
            return null;
        }
    }

    public async Task<GetProfilesResponse?> GetMatchedActiveUserAsync(string endpoint, string queryParams)
    {
        try
        {
            var response = await _apiService.GetAsync<GetProfilesResponse>(endpoint, queryParams);
            return response;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error during swipe operation: {ex.Message}", ex);
            return null;
        }
    }

    public async Task<GetMessagesResponse?> GetMessagesAsync(string endpoint, string queryParams)
    {
        try
        {
            var response = await _apiService.GetAsync<GetMessagesResponse>(endpoint, queryParams);
            return response;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error during swipe operation: {ex.Message}", ex);
            return null;
        }
    }

    public async Task<GetBasicUserMessageResponse?> GetUserMessageAsync(string endpoint, string queryParams)
    {
        try
        {
            var response = await _apiService.GetAsync<GetBasicUserMessageResponse>(endpoint, queryParams);
            return response;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error during swipe operation: {ex.Message}", ex);
            return null;
        }
    }

    public async Task<SendMessageResponse?> SendMessagesAsync(SendMessageRequest request)
    {
        try
        {
            var response = await _apiService.PostAsync<SendMessageResponse>(EnvironmentsExtensions.ENDPOINT_SEND_MESSAGES, request);
            return response;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error during swipe operation: {ex.Message}", ex);
            return null;
        }
    }
}