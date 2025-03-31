using Android.Views;
using System.Diagnostics;
using AWindow = Android.Views.Window;

namespace LonerApp.Platforms.Android.Services
{
    public class SystemStyleManager : ISystemStyleManager
    {
        public void SetNavigationBarColor(string hexColorStart, string hexColorEnd, bool isAnimated = false)
        {
            AWindow currentWindow = GetCurrentWindow();
            if (currentWindow == null)
                return;
            try
            {
                // Convert  string hexColor to Android.Graphics.Color
                var colorStart = global::Android.Graphics.Color.ParseColor(hexColorStart);
                var colorEnd = global::Android.Graphics.Color.ParseColor(hexColorEnd);

                if (OperatingSystem.IsAndroidVersionAtLeast(35))
                {

                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error when set navigation bar color " + ex.Message);
            }
        }

        public void SetStatusBarColor(string hexColorStart)
        {
            AWindow window = GetCurrentWindow();
            if (window == null)
            {
                return;
            }
            try
            {
                var startColor = global::Android.Graphics.Color.ParseColor(hexColorStart);
                window.DecorView.SystemUiVisibility |= (StatusBarVisibility)SystemUiFlags.LightStatusBar;
                window.SetStatusBarColor(startColor);
            }
            catch (Exception)
            {
            }
        }

        AWindow GetCurrentWindow()
        {
            var currentMainPage = AppHelper.CurrentMainPage;
            // Get current Window of dialog in currentMainPage if it is exist.
            // get number modal of current page in stack navigation
            if (currentMainPage != null && currentMainPage.Navigation.ModalStack.Count > 0)
            {
                // get fragmentManager: is method in FragmentActivity to get FragmentManager
                var fragmentManager = (Platform.CurrentActivity as MauiAppCompatActivity)?.SupportFragmentManager;

                //get first dialog fragment in all current fragments
                var targetFragment = fragmentManager?.Fragments
                                    .OfType<AndroidX.Fragment.App.DialogFragment>()
                                    .FirstOrDefault();

                return targetFragment?.Dialog?.Window;
            }

            var window = Platform.CurrentActivity?.Window;
            if (window == null)
            {
                return null;
            }
            try
            {
                //remove transparent for status bar or  navigation bar
                window.ClearFlags(WindowManagerFlags.TranslucentStatus);

                //add flag for set background color for status bar or navigation bar
                window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

                return window;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error when get current window " + ex.Message);
                return null;
            }
        }
    }
}
