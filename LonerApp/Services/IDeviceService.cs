namespace LonerApp.Services
{
    public interface IDeviceService
    {
        Stream GetRotatedImageStream(string path, bool isRotate = false, int maxDimension = 0);
        Task<bool> RegisterForPushNotificationsAsync();

        double GetStatusBarHeight();

        double GetNavigationBarHeight();

        bool IsSoftKeyboardVisible(Android.Views.View view);

        void HideKeyboard();

        void SetResizeKeyboardInput();
    }
}