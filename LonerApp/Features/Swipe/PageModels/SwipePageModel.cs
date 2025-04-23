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
            Users = [.. data?.User?.Items ?? []];
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
            if (newValue is not UserProfileResponse currentItem)
                return;

            if (Users.IndexOf(currentItem) < PrefetchThreshold)
                return;

            _ = Task.Run(async () =>
            {
                await PrefetchUsersAsync();
                await Task.Delay(100);
            });
        }

        private int _currentPage = 1;
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
                _currentPage++;
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
            return data?.User?.Items ?? Enumerable.Empty<UserProfileResponse>();
        }
    }
}