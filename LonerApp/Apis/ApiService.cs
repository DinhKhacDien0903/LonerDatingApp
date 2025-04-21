
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace LonerApp.Apis;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    public ApiService()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };
        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(GetBaseUrl())
        };
    }

    public async Task DeleteAsync(string endpoint, string queryParams = "")
    {
        try
        {
            var url = string.IsNullOrEmpty(queryParams) ? endpoint : $"{endpoint}?{queryParams}";
            var response = await _httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException($"HTTP Error: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new ApiException($"Error calling API: {ex.Message}", ex);
        }
    }

    public async Task<T?> GetAsync<T>(string endpoint, string queryParams = "")
    {
        try
        {
            // var url = string.IsNullOrEmpty(queryParams) ? endpoint : $"{endpoint}?{queryParams}";
            var url = string.IsNullOrEmpty(queryParams) ? endpoint : $"{endpoint}?PaginationRequest.UserId={queryParams}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var check = await response.Content.ReadAsStringAsync();
            System.Console.WriteLine($"Response: {check}");
            if (string.IsNullOrEmpty(check))
            {
                return default;
            }

            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException($"HTTP Error: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new ApiException($"Error calling API: {ex.Message}", ex);
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException($"HTTP Error: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new ApiException($"Error calling API: {ex.Message}", ex);
        }
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException($"HTTP Error: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new ApiException($"Error calling API: {ex.Message}", ex);
        }
    }

    private string GetBaseUrl() => Enviroments.URl_SERVER_HTTPS_EMULATOR;
}
public class ApiException : Exception
{
    public ApiException(string message, Exception innerException = null)
        : base(message, innerException)
    {
    }
}