using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace LonerApp
{
    [Activity(MainLauncher = true,
        LaunchMode = LaunchMode.SingleTop,
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            this.Window.RequestFeature(WindowFeatures.ActionBar);
            this.SetTheme(Resource.Style.MainTheme);
            base.OnCreate(savedInstanceState);
            //HideSystemUI();
        }

        private void HideSystemUI()
        {
            var window = Platform.CurrentActivity?.Window;;
            if (window != null)
            {
                window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
                {
                    window.DecorView.WindowInsetsController?.Hide(WindowInsets.Type.StatusBars() | WindowInsets.Type.NavigationBars());
                }
                else
                {
                    window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
                        SystemUiFlags.HideNavigation |
                        SystemUiFlags.ImmersiveSticky |
                        SystemUiFlags.Fullscreen);
                }
            }
        }
    }
}
