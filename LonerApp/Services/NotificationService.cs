using Microsoft.AspNetCore.SignalR.Client;
using Plugin.LocalNotification;

namespace LonerApp.Services
{
    public class NotificationService : INotificationService
    {
        private readonly HubConnection _connection;
        private static int _notificationId = 0;
        public NotificationService()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(Environments.URl_SERVER_HTTPS_DEVICE_4G_NOTIFICATION_HUB_MAC, options =>
                {
                    options.HttpMessageHandlerFactory = _ => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                    };
                    options.AccessTokenProvider = async () => await JWTHelper.GetValidAccessToken();
                })
                .WithAutomaticReconnect()
                .Build();

            _connection.On<NotificationResponse>("ReceiveNotification", HandleNotificationAsync);

            _connection.On<string, string>("UserNotConnected", async (method, message) =>
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await AlertHelper.ShowErrorAlertAsync(message);
                    AppHelper.SetMainPage(new LoadingPage());
                });
            });
        }

        public async Task StartAsync()
        {
            if (_connection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await _connection.StartAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SignalR connection error: {ex.Message}");
                    await Task.Delay(5000);
                    await StartAsync();
                }
            }
        }

        public async Task StopAsync()
        {
            if (_connection.State != HubConnectionState.Disconnected)
            {
                await _connection.StopAsync();
                Console.WriteLine("SignalR Hub disconnected.");
            }
        }

        private async Task HandleNotificationAsync(NotificationResponse notification)
        {
            try
            {
                var notificationData = new
                {
                    Type = notification.Type.ToString(),
                    RelatedId = notification.RelatedId,
                    UserId = notification.SenderId,
                    UserName = notification.Title,
                };

                var returningData = System.Text.Json.JsonSerializer.Serialize(notification);
                var request = new NotificationRequest
                {
                    NotificationId = _notificationId,
                    Title = notification.Type == 2 ? notification.Title : "Notification",
                    Subtitle = notification.Subtitle,
                    Description = (notification?.Messeage ?? "").Contains("https://") ? "Hình ảnh" : notification?.Messeage ?? "You have a new notification",
                    BadgeNumber = 42,
                    ReturningData = returningData,
                    Image = new NotificationImage
                    {
                        FilePath = (notification?.Messeage ?? "").Contains("https://") ? await DownloadImageAsync(notification?.Messeage ?? "", notification?.Id ?? "") : null,
                    },
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now.AddSeconds(1),
                    }
                };
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await LocalNotificationCenter.Current.Show(request);
                    _notificationId++;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error showing notification: {ex.Message}");
            }
        }

        private async Task<string> DownloadImageAsync(string imageUrl, string notificationId)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return null;
                }
                var fileName = $"avatar_{notificationId}.jpg";
                var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
                if (File.Exists(filePath))
                {
                    return filePath;
                }

                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                using var httpClient = new HttpClient(handler);
                var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);

                await File.WriteAllBytesAsync(filePath, imageBytes);

                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading image from {imageUrl}: {ex.Message}");
                return null;
            }
        }

        public async Task CleanUpCachedImagesAsync()
        {
            try
            {
                var cacheDir = FileSystem.CacheDirectory;
                var files = Directory.GetFiles(cacheDir, "avatar_*.jpg");
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.LastWriteTime < DateTime.Now.AddDays(-7))
                    {
                        File.Delete(file);
                    }
                }
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cleaning up cached images: {ex.Message}");
            }
        }
    }
}