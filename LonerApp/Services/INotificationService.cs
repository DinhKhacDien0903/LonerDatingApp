namespace LonerApp.Services
{
    public interface INotificationService
    {
        Task StartAsync();
        Task StopAsync();
        Task CleanUpCachedImagesAsync();
    }
}