using CommunityToolkit.Mvvm.Input;
using Plugin.LocalNotification;

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
            LocalNotificationCenter.Current.NotificationActionTapped += Current_NotificationActionTapped;
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
            //await _navigationOtherShell.NavigateToAsync<EmailAuthor>(param: "user11@test.com", isPushModal: false);

            var request = new NotificationRequest
            {
                NotificationId = 1337,
                Title = "Subscribe to my channel",
                Subtitle = "Hello",
                Description = "It's me",
                BadgeNumber = 42,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(5),
                    NotifyRepeatInterval = TimeSpan.FromDays(1),
                }
            };

            await LocalNotificationCenter.Current.Show(request);
        }

        [RelayCommand]
        async Task OnPhoneSignInAsync(object param)
        {
            await _navigationOtherShell.NavigateToAsync<PhoneNumberAuthor>(isPushModal: true);
        }
    }
}