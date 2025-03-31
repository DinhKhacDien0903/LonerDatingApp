using Android.Content.PM;
namespace LonerApp.Services
{
    public class OrientationService : IOrientationService
    {
        public void Portrait()
        {
            Platform.CurrentActivity.RequestedOrientation = ScreenOrientation.Portrait;
        }
    }
}
