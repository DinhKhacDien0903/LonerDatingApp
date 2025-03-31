using Android.Content;
using Settings = Android.Provider.Settings;
using Application = Android.App.Application;
namespace LonerApp.Services
{
    public class OpenSetting : IOpenSetting
    {
        public void OpenSettingScreen()
        {
            var intent = new Intent(Settings.ActionSettings);
            intent.AddFlags(ActivityFlags.NewTask);
            Application.Context.StartActivity(intent);
        }
    }
}