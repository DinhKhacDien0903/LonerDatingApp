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
            var check = 1;
            _navigationOtherShell = navigationOtherShell;
        }

        [RelayCommand]
        async Task OnSignIn()
        {
            UserSetting.Set(StorageKey.IsLoggingIn, "true");
            await NavigationService.PushToPageAsync<SignInPage>();
        }

        [RelayCommand]
        async Task OnGoogleSignInAsync(object param)
        {
            //await NavigationService.PushToPageAsync<EmailAuthor>(isPushModal: true);
            await _navigationOtherShell.NavigateToAsync<VerifyPhoneEmailAuthorPage>(param: "user1@test.com", isPushModal: false);
            // await NavigationService.PushToPageAsync<MainSwipePage>(isPushModal: false);
        }

        [RelayCommand]
        async Task OnPhoneSignInAsync(object param)
        {
            await NavigationService.PushToPageAsync<PhoneNumberAuthor>(isPushModal: true);
        }
    }
}