using Android.App;
using Android.Content;
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
        public static TaskCompletionSource<bool> LocationSettingsTcs { get; set; }
         public const int ACCURACY_LOCATION_REQUEST = 1001;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            this.Window.RequestFeature(WindowFeatures.ActionBar);
            this.SetTheme(Resource.Style.MainTheme);
            base.OnCreate(savedInstanceState);
            //HideSystemUI();
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == ACCURACY_LOCATION_REQUEST)
            {
                LocationSettingsTcs?.TrySetResult(result: resultCode == Result.Ok);
            }
        }
        private void HideSystemUI()
        {
            var window = Platform.CurrentActivity?.Window; ;
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
