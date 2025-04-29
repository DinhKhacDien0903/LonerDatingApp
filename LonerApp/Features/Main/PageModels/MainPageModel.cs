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
            // LocalNotificationCenter.Current.NotificationActionTapped += Current_NotificationActionTapped;
        }

        [RelayCommand]
        async Task OnSignInAsync()
        {
            UserSetting.Set(StorageKey.IsLoggingIn, "true");
            await Task.Delay(50);
            await _navigationOtherShell.NavigateToAsync<SignInPage>(isPushModal: false);
        }

        private void Current_NotificationActionTapped(Plugin.LocalNotification.EventArgs.NotificationActionEventArgs e)
        {
            if (e.IsDismissed)
            {

            }
            else if (e.IsTapped)
            {

            }
        }

        [RelayCommand]
        async Task OnGoogleSignInAsync(object param)
        {
            await _navigationOtherShell.NavigateToAsync<EmailAuthor>(param: "user11@test.com", isPushModal: false);
            // await _navigationOtherShell.NavigateToAsync<VerifyPhoneEmailAuthorPage>(param: "user17@test.com", isPushModal: false);
        }

        [RelayCommand]
        async Task OnPhoneSignInAsync(object param)
        {
            await _navigationOtherShell.NavigateToAsync<PhoneNumberAuthor>(isPushModal: true);
        }
    }
}