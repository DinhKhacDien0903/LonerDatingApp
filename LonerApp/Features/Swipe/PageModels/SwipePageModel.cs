using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using LonerApp.Features.Services;
using System.Collections.ObjectModel;
using System.Linq;
using static Android.Icu.Util.LocaleData;
using System.Text.Json;

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

        public SwipePageModel(INavigationService navigationService, ISwipeService swipeService)
            : base(navigationService, true)
        {
            IsVisibleNavigation = true;
            _swipeService = swipeService;
        }

        public override async Task InitAsync(object? initData)
        {
            if (initData is string UserId)
                _currentUserId = UserId.Trim();
            IsNeedLoadUsersData = false;
            await base.InitAsync(initData);
        }

        public override async Task LoadDataAsync()
        {
            _currentUserId = !string.IsNullOrEmpty(_currentUserId) ? _currentUserId : UserSetting.Get(StorageKey.UserId);
            string queryParams = $"{EnvironmentsExtensions.QUERY_PARAMS_PAGINATION_REQUEST}{_currentUserId}";
            var data = await _swipeService.GetProfilesAsync(EnvironmentsExtensions.ENDPOINT_GET_PROFILES, queryParams);
            _hasMoreUsers = data?.User?.Items.Any() ?? false;
            Users = [.. data?.User?.Items ?? []];
            countUser = Users.Count;
            _currentPage++;
            await base.LoadDataAsync();
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
            await NavigationService.PushToPageAsync<FilterMapPage>();
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
            Users.Remove(user);

            if (Users.Count <= PrefetchThreshold && !_isLoading && _hasMoreUsers)
            {
                await PrefetchUsersAsync();
            }
            else if (Users.Count == 0 && !_hasMoreUsers)
            {
                await Shell.Current.DisplayAlert("Info", "No more users to swipe", "OK");
            }
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
                await Task.Delay(1);
            }

            Users.Remove(user);

            if (Users.Count <= PrefetchThreshold && !_isLoading && _hasMoreUsers)
            {
                await PrefetchUsersAsync();
            }
            else if (Users.Count == 0 && !_hasMoreUsers)
            {
                await Shell.Current.DisplayAlert("Info", "No more users to swipe", "OK");
            }
            IsBusy = false;
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
            await NavigationService.PushToPageAsync<DetailProfilePage>(param: (param as UserProfileResponse)?.Id, isPushModal: true);
            await Task.Delay(100);
            IsBusy = false;
        }

        public void OnTopItemPropertyChanged(object newValue)
        {
            //if (countUser <= 0 || newValue is not UserProfileResponse currentItem)
            //    return;

            //var ci = (Users.IndexOf(currentItem));
            //var temp = countUser - PrefetchThreshold;
            //if ((Users.IndexOf(currentItem) < (countUser - PrefetchThreshold)) || !_hasMoreUsers)
            //    return;

            //_ = Task.Run(async () =>
            //{
            //    await PrefetchUsersAsync();
            //    await Task.Delay(100);
            //});
            if (countUser <= 0 || newValue is not UserProfileResponse currentItem || !_hasMoreUsers)
                return;

            // Prefetch khi còn 5 user hoặc ít hơn
            if (countUser <= PrefetchThreshold)
            {
                _ = Task.Run(async () =>
                {
                    await PrefetchUsersAsync();
                    await Task.Delay(100);
                });
            }
        }

        private bool _isLoading = false;
        private const int PageSize = 10;
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
            //https://localhost:7165/api/Swipe/profiles?PaginationRequest.PageNumber=2&PaginationRequest.ValidPageSize=10&PaginationRequest.UserId=8cb3f197-f8ff-44ef-a65a-daf0b6a506c8
            string queryParams = $"?PaginationRequest.PageNumber={pageNumber}&PaginationRequest.ValidPageSize={PageSize}&PaginationRequest.UserId={_currentUserId}";
            //string queryParams = $"{EnvironmentsExtensions.QUERY_PARAMS_PAGINATION_REQUEST}{_currentUserId}";
            var data = await _swipeService.GetProfilesAsync(EnvironmentsExtensions.ENDPOINT_GET_PROFILES, queryParams);
            _hasMoreUsers = data?.User?.Items.Any() ?? false;
            countUser = Users.Count;
            return data?.User?.Items ?? [];
        }
    }
}