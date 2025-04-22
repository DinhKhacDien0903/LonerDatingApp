using System.Diagnostics;

namespace LonerApp.Helpers
{
    public static class AppHelper
    {
        private const string HexColorStatusBarStart = "#ffffff";

        public static Page CurrentMainPage
        {
            get
            {
                try
                {
                    var currentPage = Application.Current?.Windows.FirstOrDefault(w => w.Page != null)?.Page;
                    return currentPage ?? throw new InvalidOperationException("Current page is null, ensure the app has valid page!");
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(("Error when get current page " + ex.Message));
                    return null;
                }
            }
        }

        public static void SetMainPage(Page newPage)
        {
            try
            {
                if (newPage == null)
                    throw new ArgumentNullException(nameof(newPage), "New MainPage cannot be null.");

                var app = Application.Current;
                if (app == null)
                    throw new InvalidOperationException("Current Application instance available.");

                if (app.Windows.Count == 0)
                    throw new InvalidOperationException("No Windows available in the current Application.");
#if ANDROID
                if (OperatingSystem.IsAndroidVersionAtLeast(35) && newPage is not AppShell)
                {
                    var bottomPadding = ServiceHelper.GetService<IDeviceService>().GetNavigationBarHeight();
                    var topPadding = ServiceHelper.GetService<IDeviceService>().GetStatusBarHeight();
                    newPage.Padding = new Thickness(0, topPadding, 0, bottomPadding);
                }
#endif

                app.Windows[0].Page = new NavigationPage(newPage);
            }
            catch(Exception e)
            {
                throw;
            }
        }

        public static void RefreshApp()
        {
            //if ((App.Current as App).IsAppSleepingOrCovered)
            //    return;
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (CurrentMainPage is AppShell appShell)
                    await appShell?.RemoveRootAsync();
                SetMainPage(new AppShell()); // REQUIRE RUN MAIN THREAD
                ServiceHelper.GetService<ISystemStyleManager>().SetStatusBarColor(HexColorStatusBarStart);
            });
        }
    }
}