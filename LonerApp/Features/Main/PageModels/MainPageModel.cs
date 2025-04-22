using CommunityToolkit.Mvvm.Input;

namespace LonerApp.PageModels
{
    public partial class MainPageModel : BasePageModel
    {
        public MainPageModel(INavigationService navigationService)
            : base(navigationService, true)
        {

        }

        [RelayCommand]
        async Task OnSignIn()
        {
            await NavigationService.PushToPageAsync<SignInPage>();
        }

        [RelayCommand]
        async Task OnGoogleSignInAsync(object param)
        {
            await NavigationService.PushToPageAsync<EmailAuthor>(isPushModal: true);
            // await NavigationService.PushToPageAsync<MainSwipePage>(isPushModal: false);
        }

        [RelayCommand]
        async Task OnPhoneSignInAsync(object param)
        {
            await NavigationService.PushToPageAsync<PhoneNumberAuthor>(isPushModal: true);
        }
    }
}