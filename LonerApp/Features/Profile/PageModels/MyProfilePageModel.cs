using CommunityToolkit.Mvvm.Input;

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
    public MyProfilePageModel(INavigationService navigationService)
        : base(navigationService, true)
    {
        IsVisibleNavigation = true;
    }

    public override async Task InitAsync(object? initData)
    {
        await base.InitAsync(initData);
    }

    public override Task LoadDataAsync()
    {
        ImageProfile = "bbbb.jpeg";
        return base.LoadDataAsync();
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
        await NavigationService.PushToPageAsync<EditProfilePage>();
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