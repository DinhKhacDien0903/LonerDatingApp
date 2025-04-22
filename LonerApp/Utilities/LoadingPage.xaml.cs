namespace LonerApp.Utilities;

public partial class LoadingPage : BasePage
{
    private bool _isLoggedIn;
    private bool _isAccountSetup;
    private bool _loggedIn;
    public bool IsAppSleep = false;

    public LoadingPage()
    {
        InitializeComponent();
    }

    public override void OnResume()
    {
        base.OnResume();
        IsAppSleep = false;
        if (_loggedIn)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await this.HandleSelectCountryAsync();
            });
        }
    }

    protected override void OnAppearing()
    {
        ServiceHelper.GetService<ISystemStyleManager>().SetStatusBarColor("#ffffff");
        base.OnAppearing();
        //if (!_isLoggedIn)
        DoLogin();
    }

    private void DoLogin()
    {
        //UserSetting.Remove("IsLoggedIn");
        var stopRestoreLastSession = false;
        var stopInitLoggedIn = false;
        var isLoggedIn = UserSetting.Get(StorageKey.IsLoggedIn);

        if (string.IsNullOrEmpty(isLoggedIn))
            _isLoggedIn = false;
        else
            _isLoggedIn = Convert.ToBoolean(isLoggedIn);

        //RunProgreessBarAsync(new FakeProgressConfig(), () => stopRestoreLastSession).ConfigureAwait(false);
        Task.Run(async () =>
        {
            //await MainThread.InvokeOnMainThreadAsync(() =>
            //{
            //    indicator.IsRunning = true;
            //    LoadingText.IsVisible = true;
            //});

            if (!_isLoggedIn)
            {
                stopRestoreLastSession = true;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateProgressLayout(0);
                });

                await Navigation.PushAsync(new MainPage());
                indicator.IsRunning = false;
                LoadingText.IsVisible = false;
            }
            if (_isLoggedIn)
            {
                var isAccountSetup = UserSetting.Get(StorageKey.IsAccountSetup);
                if (string.IsNullOrEmpty(isAccountSetup))
                    _isAccountSetup = false;
                else
                    _isAccountSetup = Convert.ToBoolean(isAccountSetup);

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    stopRestoreLastSession = true;
                    stopInitLoggedIn = true;
                    UpdateProgressLayout(1);
                    await Task.Delay(100);
                    _loggedIn = true;
                    if (!_isAccountSetup)
                    {
                        AppHelper.SetMainPage(new SetupNamePage(ServiceHelper.GetPageModelObservable<SetupPageModel>()));
                    }
                    else if (IsAppSleep == false)
                    {
                        await this.HandleSelectCountryAsync();
                    }
                    indicator.IsRunning = false;
                    LoadingText.IsVisible = false;
                });
            }
        });
    }

    public async Task HandleSelectCountryAsync()
    {
        await OpenMainAppAsync();
    }

    public static async Task OpenMainAppAsync()
    {
        ServiceHelper.GetService<ISystemStyleManager>().SetStatusBarColor("#003185");

        if (App.CurrentPageModel != null)
        {
            ServiceHelper.GetService<ISystemStyleManager>().SetStatusBarColor("#ffffff");
            await ServiceHelper.GetService<INavigationService>().PopPageAsync(isPopModal: true);
        }

        if (AppHelper.CurrentMainPage is not AppShell)
        {
            App.RefreshApp();
        }

    }

    private async Task RunProgreessBarAsync(FakeProgressConfig progress, Func<bool> stopFunc)
    {
        await Task.Delay((int)progress.TimeInterval);
        progress.Time += progress.TimeInterval;
        if (!progress.Expired && !stopFunc())
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var percent = progress.Percent;
                UpdateProgressLayout(percent);
            });
            await RunProgreessBarAsync(progress, stopFunc);
        }
    }

    private void UpdateProgressLayout(double percent)
    {
        if (Math.Abs(percent) < 0.01)
            LoadingText.Text = string.Empty;
        else
            LoadingText.Text = $"Loading ...{(int)(percent * 100)}%";
    }
}

class FakeProgressConfig
{
    private static int GlobalKey;
    private int _localKey = ++GlobalKey;

    public bool Expired => _localKey != GlobalKey;
    public double TimeConstant { get; set; } = 5000;
    public double TimeInterval { get; set; } = 300;
    public double Time { get; set; }
    public double Percent => 1 - Math.Exp(-1 * Time / TimeConstant);
}