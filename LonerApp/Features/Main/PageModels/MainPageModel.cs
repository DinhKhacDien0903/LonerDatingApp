using CommunityToolkit.Mvvm.Input;

namespace LonerApp.PageModels
{
    public partial class MainPageModel : BasePageModel
    {
        private readonly INavigationOtherShellService _navigationOtherShell;
        public MainPageModel(INavigationService navigationService,
            INavigationOtherShellService navigationOtherShell)
            : base(navigationService, true)
        {
            _navigationOtherShell = navigationOtherShell;
        }

        [RelayCommand]
        async Task OnSignInAsync()
        {
            UserSetting.Remove("IsLoggedIn");
            UserSetting.Set(StorageKey.IsLoggingIn, "true");
            await Task.Delay(50);
            await _navigationOtherShell.NavigateToAsync<SignInPage>(isPushModal: false);
        }

        [RelayCommand]
        async Task OnSignUpAsync()
        {
            UserSetting.Remove("IsLoggedIn");
            UserSetting.Set(StorageKey.IsLoggingIn, "false");
            await Task.Delay(50);
            await _navigationOtherShell.NavigateToAsync<SignUpPage>(isPushModal: false);
        }

        [RelayCommand]
        async Task OnGoogleSignInAsync(object param)
        {
            await _navigationOtherShell.NavigateToAsync<EmailAuthor>(isPushModal: false);
        }

        [RelayCommand]
        async Task OnPhoneSignInAsync(object param)
        {
            await _navigationOtherShell.NavigateToAsync<PhoneNumberAuthor>(isPushModal: true);
        }

        [RelayCommand]
        async Task OnGoogleSignUpAsync(object param)
        {
            await _navigationOtherShell.NavigateToAsync<EmailAuthor>(isPushModal: false);
        }

        [RelayCommand]
        async Task OnPhoneNumberSignUpAsync(object param)
        {
            await _navigationOtherShell.NavigateToAsync<PhoneNumberAuthor>(isPushModal: true);
        }
    }
}