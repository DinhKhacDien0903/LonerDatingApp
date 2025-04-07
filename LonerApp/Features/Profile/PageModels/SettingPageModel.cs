using CommunityToolkit.Mvvm.Input;

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

        public SettingPageModel(INavigationService navigationService)
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
            //await NavigationService.PushToPageAsync<SettingPage>();
            await Task.Delay(100);
            IsBusy = false;
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