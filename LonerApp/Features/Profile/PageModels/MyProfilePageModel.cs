using CommunityToolkit.Mvvm.Input;
using LonerApp.Features.Services;

namespace LonerApp.PageModels;

public partial class MyProfilePageModel : BasePageModel
{
    [ObservableProperty]
    string _imageProfile;
    public bool IsNeedLoadUsersData = true;
    [ObservableProperty]
    private bool _isVisibleNavigation;
    [ObservableProperty]
    private bool _hasBackButton;
    [ObservableProperty]
    private string _description;
    private UserProfileDetailResponse _myProfile = new();
    private string _currentUserId = string.Empty;
    private readonly IProfileService _profileService;
    public MyProfilePageModel(
        INavigationService navigationService,
        IProfileService profileService)
        : base(navigationService, true)
    {
        _profileService = profileService;
        IsVisibleNavigation = true;
    }

    public override async Task InitAsync(object? initData)
    {
        await base.InitAsync(initData);
    }

    public override async Task LoadDataAsync()
    {
        _currentUserId = UserSetting.Get(StorageKey.UserId);
        string queryParams = $"{EnvironmentsExtensions.QUERY_PARAMS_USER_ID}{_currentUserId}";
        _myProfile = (await _profileService.GetProfileDetailAsync(EnvironmentsExtensions.ENDPOINT_GET_PROFILE_DETAIL, queryParams))?.UserDetail ?? new();
        ImageProfile = _myProfile?.AvatarUrl ?? "";
        Description = $"{_myProfile?.UserName ?? " "}, {_myProfile?.Age ?? 18}";
        await base.LoadDataAsync();
    }

    [RelayCommand]
    async Task OnGotoSettingAsync(object param)
    {
        if (GotoSettingCommand.IsRunning || IsBusy)
            return;
        IsBusy = true;
        await NavigationService.PushToPageAsync<SettingPage>();
        await Task.Delay(100);
        IsBusy = false;
    }

    [RelayCommand]
    async Task OnGotoEditProfileAsync(object param)
    {
        if (GotoEditProfileCommand.IsRunning || IsBusy)
            return;
        IsBusy = true;
        await NavigationService.PushToPageAsync<EditProfilePage>(param: _myProfile);
        await Task.Delay(100);
        IsBusy = false;
    }

    [RelayCommand]
    async Task OnGotoGooglePlayAsync(object param)
    {
        if (GotoGooglePlayCommand.IsRunning || IsBusy)
            return;
        IsBusy = true;
        //await NavigationService.PopPageAsync<EditProfilePage>();
        await Task.Delay(100);
        IsBusy = false;
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
}