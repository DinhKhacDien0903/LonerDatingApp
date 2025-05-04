using Android.Hardware.Usb;
using CommunityToolkit.Mvvm.Input;
using LonerApp.Features.Services;
using LonerApp.Helpers;
using System.Runtime.Caching.Hosting;

namespace LonerApp.PageModels
{
    public partial class SettingPageModel : BasePageModel
    {
        public bool IsNeedLoadUsersData = true;
        [ObservableProperty]
        private bool _isVisibleNavigation;
        [ObservableProperty]
        private bool _hasBackButton;
        [ObservableProperty]
        private string _locationEditorValue;
        [ObservableProperty]
        private string _toAgeEditorValue;
        [ObservableProperty]
        private string _fromAgeEditorValue;
        private IProfileService _profileService;
        private IAuthorService _authorService;
        public SettingPageModel(
            INavigationService navigationService,
            IProfileService profileService,
            IAuthorService authorService)
            : base(navigationService, true)
        {
            IsVisibleNavigation = true;
            _profileService = profileService;
            _authorService = authorService;
        }

        public override async Task InitAsync(object? initData)
        {
            await base.InitAsync(initData);
        }

        public override Task LoadDataAsync()
        {
            return base.LoadDataAsync();
        }

        [RelayCommand]
        async Task OnGotoSetupPhoneNumberAsync(object param)
        {
            if (GotoSetupPhoneNumberCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            await NavigationService.PushToPageAsync<PhoneNumberAuthor>();
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnGotoSetupEmailAsync(object param)
        {
            if (GotoSetupEmailCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            await NavigationService.PushToPageAsync<EmailAuthor>();
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnGotoSetupShowGenderAsync(object param)
        {
            if (GotoSetupShowGenderCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            await NavigationService.PushToPageAsync<SetupShowGenderForMe>();
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnDonePressedAsync(object param)
        {
            if (DonePressedCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            //await NavigationService.PushToPageAsync<SettingPage>();
            await Task.Delay(100);
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnLogoutPressedAsync(object param)
        {
            if (LogoutPressedCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            try
            {
                if (await AlertHelper.ShowConfirmationAlertAsync("Bạn có chắc chắn muốn đăng xuất?","Xác nhận."))
                {
                    var request = new LogoutRequest
                    {
                        UserId = UserSetting.Get(StorageKey.UserId) ?? "",
                        RefreshToken = UserSetting.Get(StorageKey.RefreshToken) ?? ""
                    };

                    var response = await _authorService.LogoutAsync(request);
                    if (response.IsSuccess)
                    {
                        UserSetting.Remove("UserId");
                        UserSetting.Remove("RefreshToken");
                        UserSetting.Remove("IsLoggedIn");
                        await (Shell.Current as AppShell)?.RemoveRootAsync();
                        await Task.Delay(100);
                        AppHelper.SetMainPage(new MainPage());
                    }
                }

                await Task.Delay(100);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Logout error: {e.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task OnDeleteAccountPressedAsync(object param)
        {
            if (DeleteAccountPressedCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            //await NavigationService.PushToPageAsync<SettingPage>();
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
}