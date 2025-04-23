using CommunityToolkit.Mvvm.Input;
using LonerApp.Features.Services;
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
        private ObservableCollection<UserProfileResponse> _users = new();
        private static string _currentUserId = string.Empty;

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
            await Task.Delay(100);
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
            await Task.Delay(100);
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnOpenDetailProfileAsync(object param)
        {
            if (OpenDetailProfileCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            IsNeedLoadUsersData = false;
            await NavigationService.PushToPageAsync<DetailProfilePage>(param: (param as UserProfileResponse)?.Id, isPushModal: true);
            await Task.Delay(100);
            IsBusy = false;
        }
    }
}