using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using LonerApp.Features.Services;

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
        private string _phoneNumberValue;
        [ObservableProperty]
        private string _emailValue;
        [ObservableProperty]
        private string _showGenderValue;
        [ObservableProperty]
        private string _fromAgeEditorValue;
        private SettingAccountResponse _settingAccount = new();
        private IProfileService _profileService;
        private IAuthorService _authorService;
        private string _currentUserId = string.Empty;
        private CancellationTokenSource cancellationToastToken = new CancellationTokenSource();
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
            _currentUserId = UserSetting.Get(StorageKey.UserId);
            var response = await _profileService.GetSettingAccountAsync(_currentUserId);
            _settingAccount = response?.SettingAccount ?? new();
            if (_settingAccount != null)
            {
                PhoneNumberValue = _settingAccount.PhoneNumber ?? string.Empty;
                EmailValue = _settingAccount.Email ?? string.Empty;
                LocationEditorValue = _settingAccount.Address ?? string.Empty;
                FromAgeEditorValue = _settingAccount.MinAge.ToString();
                ToAgeEditorValue = _settingAccount.MaxAge.ToString();
                ShowGenderValue = _settingAccount.ShowGender ? "Nam" : "Nữ";
            }

            await base.InitAsync(initData);
        }

        public override async Task LoadDataAsync()
        {
            await base.LoadDataAsync();
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
            bool isUpdateSuccess = (await _profileService.UpdateUserSettingAsync(new UpdateUserSettingRequest
            {
                EditRequest = new EditSettingAccountRequest
                {
                    UserId = _currentUserId,
                    PhoneNumber = PhoneNumberValue,
                    Email = EmailValue,
                    Address = LocationEditorValue,
                    MinAge = int.Parse(FromAgeEditorValue),
                    MaxAge = int.Parse(ToAgeEditorValue),
                    ShowGender = ShowGenderValue == "Nam"
                }
            }))?.IsSuccess ?? false;
            if (isUpdateSuccess)
            {
                await ShowToast("Cập nhật thành công");
            }
            else
            {
                await ShowToast("Cập nhật thất bại");
            }
            await Task.Delay(100);
            await NavigationService.PopPageAsync();
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
        async Task OnLogoutPressedAsync(object param)
        {
            if (LogoutPressedCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            try
            {
                if (await AlertHelper.ShowConfirmationAlertAsync("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận."))
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
                        UserSetting.Remove("AccessToken");
                        UserSetting.Remove("IsLoggedIn");
                        var apiService = ServiceHelper.GetService<IApiService>();
                        await apiService.ResetAsync();
                        await (Shell.Current as AppShell)?.RemoveRootAsync();
                        await Task.Delay(200);
                        AppHelper.SetMainPage(new MainPage());
                        await Task.Delay(100);
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