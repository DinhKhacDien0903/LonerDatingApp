using CommunityToolkit.Mvvm.Input;
using LonerApp.Apis;
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

        public SwipePageModel(INavigationService navigationService, ISwipeService swipeService)
            : base(navigationService, true)
        {
            IsVisibleNavigation = true;
            _swipeService = swipeService;
        }

        public override async Task InitAsync(object? initData)
        {
            await base.InitAsync(initData);
            IsNeedLoadUsersData = false;
        }

        public override async Task LoadDataAsync()
        {
            //TODO: Get userId in cache
            string queryParams = $"{EnvironmentsExtensions.QUERY_PARAMS_PAGINATION_REQUEST}60ada144-f342-46b1-b6a5-0ae41ee83740";
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
            await NavigationService.PushToPageAsync<DetailProfilePage>(param: (param as UserProfileResponse)?.Id ,isPushModal: true);
            await Task.Delay(100);
            IsBusy = false;
        }
    }
}