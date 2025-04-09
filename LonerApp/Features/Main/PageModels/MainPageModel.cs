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
            //await NavigationService.PushToPageAsync<EmailAuthor>(isPushModal: true);
            //await NavigationService.PushToPageAsync<MainSwipePage>(isPushModal: false);
            try
            {
                // var http = new HttpClient();
                // string BaseAddress =
                //    DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5000" : "http://localhost:5000";
                // var response = await http.GetAsync("http://10.0.2.2:5250/api/User/getInfor");

                // var data = await response.Content.ReadAsStringAsync();
                await OnCallApiBtnClicked();
                App.RefreshApp();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [RelayCommand]
        async Task OnPhoneSignInAsync(object param)
        {
            await NavigationService.PushToPageAsync<PhoneNumberAuthor>(isPushModal: true);
        }

        private async Task OnCallApiBtnClicked()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            //192.168.1.2
            //var httpClient = new HttpClient(handler);

            //var baseUrl = DeviceInfo.Platform == DevicePlatform.Android
            //                ? "https://10.0.2.2:7072"
            //                : "https://localhost:7072";
            var _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri("https://192.168.43.14:7072/");
            var userId = "24150eab-05db-4bc3-b33e-084026aaa380";
            var request = new HttpRequestMessage(HttpMethod.Get,$"api/User/getInfor?userId={userId}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            Console.WriteLine(data);
        }
    }
}