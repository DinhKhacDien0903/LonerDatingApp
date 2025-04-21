using System;

namespace LonerApp.Apis;

public interface IApiService
{
    Task<T?> GetAsync<T>(string endpoint, string queryParams = "");
    Task<T?> PostAsync<T>(string endpoint, object data);
    Task<T?> PutAsync<T>(string endpoint, object data);
    Task DeleteAsync(string endpoint, string queryParams = "");
}