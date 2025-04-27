using Android.Content;
using Application = Android.App.Application;
using Settings = Android.Provider.Settings;

namespace LonerApp.Services
{
    public class LocalNotificationService : ILocalNotificationService
    {
        public void OpenNotificationSetting()
        {
            var intent = new Intent();
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                intent.SetAction(Settings.ActionAppNotificationSettings);
                intent.PutExtra(Settings.ExtraAppPackage, Application.Context.PackageName);
            }
            else
            {
                intent.SetAction("android.settings.APP_NOTIFICATION_SETTINGS");
                intent.PutExtra("app_package", Application.Context.PackageName);
                intent.PutExtra("app_uid", Application.Context.ApplicationInfo.Uid);
            }

            intent.SetFlags(ActivityFlags.NewTask);
            Application.Context.StartActivity(intent);
        }
    }
}
