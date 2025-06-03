using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using LonerApp.Features.Services;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;

namespace LonerApp.PageModels
{
    public partial class SwipePageModel : BasePageModel
    {
        private readonly ISwipeService _swipeService;
        public bool IsNeedLoadUsersData = true;
        [ObservableProperty]
        private string _entryValue = string.Empty;
        [ObservableProperty]
        private string _errorTextValue = string.Empty;
        [ObservableProperty]
        private bool _isShowError;
        [ObservableProperty]
        private bool _isContinue;
        [ObservableProperty]
        private bool _isVisibleNavigation;
        [ObservableProperty]
        private bool _hasBackButton;
        [ObservableProperty]
        public int _topItem;
        [ObservableProperty]
        private ObservableCollection<UserProfileResponse> _users = new();
        private static string _currentUserId = string.Empty;
        private CancellationTokenSource cancellationToastToken = new CancellationTokenSource();
        private int _currentPage = 1;
        private int countUser = 0;
        private bool _hasMoreUsers = true;
        private int _notificationId = 0;

        private readonly Services.INotificationService _notificationService;
        public SwipePageModel(INavigationService navigationService,
         ISwipeService swipeService,
         Services.INotificationService notificationService)
            : base(navigationService, true)
        {
            IsVisibleNavigation = true;
            _swipeService = swipeService;
            _notificationService = notificationService;
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(100);
                await _notificationService.StartAsync();
                //await _notificationService.CleanUpCachedImagesAsync();
            });
        }

        public override async Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();
        }

        public override async Task InitAsync(object? initData)
        {
            if (initData is string UserId)
                _currentUserId = UserId.Trim();
            else if (initData is bool)
                HasBackButton = (bool)initData;
            IsNeedLoadUsersData = false;
            await base.InitAsync(initData);
        }

        public override async Task LoadDataAsync()
        {
            try
            {
                IsBusy = true;
                _currentUserId = !string.IsNullOrEmpty(_currentUserId) ? _currentUserId : UserSetting.Get(StorageKey.UserId);
                string queryParams = $"?PaginationRequest.PageNumber={_currentPage}&PaginationRequest.PageSize={PageSize}&PaginationRequest.UserId={_currentUserId}";
                var data = await _swipeService.GetProfilesAsync(EnvironmentsExtensions.ENDPOINT_GET_PROFILES, queryParams);
                _hasMoreUsers = data?.User?.Items.Any() ?? false;
                Users = [.. data?.User?.Items ?? []];
                countUser = Users.Count;
                _currentPage++;
                await base.LoadDataAsync();
            }
            finally
            {
                IsBusy = false;
            }

        }

        [RelayCommand]
        async Task OnBackAsync(object param)
        {
            if (!IsBusy)
            {
                IsBusy = true;
                await NavigationService.PopPageAsync(isPopModal: true);
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task OnFilterPressedAsync(object param)
        {
            if (FilterPressedCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            IsNeedLoadUsersData = false;
            await NavigationService.PushToPageAsync<FilterMapPage>(param: _currentUserId);
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnDislikePressedAsync(object param)
        {
            if (DislikePressedCommand.IsRunning || IsBusy)
                return;
            if (param is not UserProfileResponse user)
                return;

            IsBusy = true;
            var swipeRequest = new SwipeRequest
            {
                SwiperId = _currentUserId,
                SwipedId = (param as UserProfileResponse)?.Id ?? "",
                Action = false
            };

            await _swipeService.SwipeAsyncAsync(swipeRequest);
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnStarPressedAsync(object param)
        {
            if (StarPressedCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            await Task.Delay(100);
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnLikePressedAsync(object param)
        {
            if (LikePressedCommand.IsRunning || IsBusy)
                return;
            if (param is not UserProfileResponse user)
                return;

            IsBusy = true;
            cancellationToastToken.TryReset();
            var swipeRequest = new SwipeRequest
            {
                SwiperId = _currentUserId,
                SwipedId = (param as UserProfileResponse)?.Id ?? "",
                Action = true
            };

            var response = await _swipeService.SwipeAsyncAsync(swipeRequest);
            if (response != null && response.IsMatch)
            {
                await ShowToast(response.Message);
                await Task.Delay(100);
            }
            IsBusy = false;
        }

        private async Task HandleNotificationAsync(NotificationResponse notification)
        {
            try
            {
                var notificationData = new
                {
                    Type = notification.Type.ToString(),
                    notification.RelatedId,
                    UserId = notification.SenderId,
                };

                var returningData = System.Text.Json.JsonSerializer.Serialize(notificationData);
                var request = new NotificationRequest
                {
                    NotificationId = _notificationId,
                    Title = notification.Type == 2 ? notification.Title : "Notification",
                    Subtitle = notification.Subtitle,
                    Description = (notification?.Messeage ?? "").Contains("https://") ? "Hình ảnh" : notification?.Messeage ?? "You have a new notification",
                    BadgeNumber = 42,
                    ReturningData = returningData,
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

        private async Task ShowToast(string content)
        {
            ToastDuration duration = ToastDuration.Short;
            double fontSize = 14;

            var toast = Toast.Make(content, duration, fontSize);

            await toast.Show(cancellationToastToken.Token);
        }

        [RelayCommand]
        async Task OnOpenDetailProfileAsync(object param)
        {
            if (OpenDetailProfileCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            cancellationToastToken.TryReset();
            IsNeedLoadUsersData = false;
            await NavigationService.PushToPageAsync<DetailProfilePage>(param: (param as UserProfileResponse)?.Id);
            await Task.Delay(100);
            IsBusy = false;
        }

        public void OnTopItemPropertyChanged(object newValue)
        {
            if (countUser <= 0 || newValue is not UserProfileResponse currentItem)
                return;

            if (Users.IndexOf(currentItem) < (countUser - PrefetchThreshold))
                return;

            _ = Task.Run(async () =>
            {
                await PrefetchUsersAsync();
                await Task.Delay(100);
            });
        }

        private bool _isLoading = false;
        private const int PageSize = 30;
        private const int PrefetchThreshold = 5;
        private const int PrefetchCount = 10;

        private async Task PrefetchUsersAsync()
        {
            if (_isLoading) return;

            _isLoading = true;
            try
            {
                var firstBatch = await FetchUsersAsync(_currentPage, PrefetchCount);

                foreach (var user in firstBatch)
                {
                    Users.Add(user);
                }
                countUser = firstBatch?.Count() > 0 ? Users.Count : countUser;
                _currentPage = firstBatch?.Count() > 0 ? _currentPage + 1 : _currentPage;
            }
            catch (Exception ex)
            {
                await AlertHelper.ShowErrorAlertAsync(ex.Message, "Lỗi");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task<IEnumerable<UserProfileResponse>> FetchUsersAsync(int pageNumber, int pageSize = PageSize)
        {
            string queryParams = $"?PaginationRequest.PageNumber={pageNumber}&PaginationRequest.PageSize={PageSize}&PaginationRequest.UserId={_currentUserId}";
            var data = await _swipeService.GetProfilesAsync(EnvironmentsExtensions.ENDPOINT_GET_PROFILES, queryParams);
            _hasMoreUsers = data?.User?.Items.Any() ?? false;
            countUser = Users.Count;
            return data?.User?.Items ?? [];
        }
    }
}