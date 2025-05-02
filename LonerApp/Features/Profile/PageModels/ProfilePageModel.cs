using CommunityToolkit.Mvvm.Input;
using LonerApp.Features.Services;
using System.Collections.ObjectModel;

namespace LonerApp.PageModels
{
    public partial class ProfilePageModel : BasePageModel
    {
        [ObservableProperty]
        ObservableCollection<string> _images = new();
        [ObservableProperty]
        ObservableCollection<string> _interests = new();
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
        private int _selectedIndex;
        [ObservableProperty]
        private bool _isCurrentOtherUser = true;
        [ObservableProperty]
        private UserProfileDetailResponse _myProfile = new();
        ContentPage? _previousPage;
        SwipePageModel? _swipePageModel;
        FilterMapPageModel? _filterPageModel;
        private readonly IProfileService _profileService;
        public ProfilePageModel(INavigationService navigationService, IProfileService profileService)
            : base(navigationService, true)
        {
            IsVisibleNavigation = true;
            _profileService = profileService;
        }

        public override async Task InitAsync(object? initData)
        {
            //TODO: Handle when user click on profile
            _previousPage = AppShell.Current?.CurrentPage as ContentPage;
            if (_previousPage != null)
            {
                _swipePageModel = _previousPage.BindingContext as SwipePageModel;
                if (_swipePageModel == null)
                    _filterPageModel = _previousPage.BindingContext as FilterMapPageModel;
            }
            IsCurrentOtherUser = _previousPage is MainSwipePage || _previousPage is FilterMapPage;
            if (initData is string userId)
            {
                MyProfile.Id = userId;
            }

            await base.InitAsync(initData);
        }

        public override async Task LoadDataAsync()
        {
            string queryParams = $"{EnvironmentsExtensions.QUERY_PARAMS_USER_ID}{MyProfile.Id}";
            try
            {
                IsBusy = true;
                MyProfile = (await _profileService.GetProfileDetailAsync(EnvironmentsExtensions.ENDPOINT_GET_PROFILE_DETAIL, queryParams))?.UserDetail ?? new();
                await Task.Delay(100);
                await InitImages();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Throw ex in ProfilePageModel: " + ex.Message);
            }
            finally
            {
                await Task.Delay(100);
                IsBusy = false;
            }

            await base.LoadDataAsync();
        }
        public async Task InitImages()
        {
            await Task.Delay(1);
            List<string> photos = [.. MyProfile?.Photos ?? []];
            foreach (var item in photos)
            {
                Images.Add(item);
            }

            await Task.Delay(100);
            _ = Task.Delay(150).ContinueWith(_ => SelectedIndex = 0);
        }

        [RelayCommand]
        async Task OnBackAsync(object param)
        {
            if (BackCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            await NavigationService.PopPageAsync(isPopModal: true);
            await Task.Delay(100);
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnCloseDetailProfileAsync(object param)
        {
            if (CloseDetailProfileCommand.IsRunning && IsBusy)
                return;

            IsBusy = true;
            await NavigationService.PopPageAsync(isPopModal: true);
            await Task.Delay(100);
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnDislikePressedAsync(object param)
        {
            //TODO: Add adnimation when dislike pressed
            if (DislikePressedCommand.IsRunning || IsBusy)
                return;

            if (param is not UserProfileDetailResponse user)
                return;

            var data = new UserProfileResponse
            {
                Id = user?.Id ?? ""
            };
            if (_swipePageModel != null)
            {
                _swipePageModel.DislikePressedCommand.Execute(data);
            }
            else if (_filterPageModel != null)
                await _filterPageModel.HandleDisLikeAsync(data);

            await OnCloseDetailProfileAsync(null);
        }

        [RelayCommand]
        async Task OnStarPressedAsync(object param)
        {
            //TODO: Add adnimation when superlike pressed
            if (StarPressedCommand.IsRunning || IsBusy)
                return;
            if (_swipePageModel != null)
            {
                _swipePageModel.StarPressedCommand.Execute(null);
            }

            await OnCloseDetailProfileAsync(null);
        }

        [RelayCommand]
        async Task OnLikePressedAsync(object param)
        {
            //TODO: Add adnimation when like pressed
            if (LikePressedCommand.IsRunning || IsBusy)
                return;

            if (param is not UserProfileDetailResponse user)
                return;

            var data = new UserProfileResponse
            {
                Id = user?.Id ?? ""
            };
            if (_swipePageModel != null)
            {
                _swipePageModel.LikePressedCommand.Execute(data);
            }
            else if (_filterPageModel != null)
                await _filterPageModel.HandleLikeAsync(data);
            await OnCloseDetailProfileAsync(null);
        }
    }
}