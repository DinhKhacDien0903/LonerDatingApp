
using LonerApp.Apis;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace LonerApp.Services
{
    public class NotificationService : INotificationService
    {
        private readonly HubConnection _connection;
        private readonly IApiService _apiService;
        public NotificationService(IApiService apiService)
        {
            _apiService = apiService;

            _connection = new HubConnectionBuilder()
                .WithUrl(Environments.URl_SERVER_HTTPS_DEVICE_WIFI_CHAT_HUB, options =>
                {
                    options.HttpMessageHandlerFactory = _ => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                    };
                    options.AccessTokenProvider = async () => await JWTHelper.GetValidAccessToken();
                })
                .WithAutomaticReconnect()
                .Build();

            // Lắng nghe sự kiện ReceiveNotification
            _connection.On<NotificationModel>("ReceiveNotification", async (notification) =>
            {
                await HandleNotificationAsync(notification);
            });
        }

        public Task StartAsync()
        {
            throw new NotImplementedException();
        }

        public Task StopAsync()
        {
            throw new NotImplementedException();
        }
    }
}