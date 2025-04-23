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
            UserSetting.Set(StorageKey.IsLoggingIn, "true");
            await Task.Delay(50);
            await _navigationOtherShell.NavigateToAsync<SignInPage>(isPushModal: false);
        }

        [RelayCommand]
        async Task OnGoogleSignInAsync(object param)
        {
            await _navigationOtherShell.NavigateToAsync<VerifyPhoneEmailAuthorPage>(param: "user1@test.com", isPushModal: false);
        }

        [RelayCommand]
        async Task OnPhoneSignInAsync(object param)
        {
            await _navigationOtherShell.NavigateToAsync<PhoneNumberAuthor>(isPushModal: true);
        }
    }
}