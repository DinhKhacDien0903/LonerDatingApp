namespace LonerApp.Services
{
    public interface IDeviceService
    {
        Stream GetRotatedImageStream(string path, bool isRotate = false, int maxDimension = 0);

        double GetStatusBarHeight();

        double GetNavigationBarHeight();
    }
}