namespace LonerApp.Services
{
    public interface  ISystemStyleManager
    {
        void SetNavigationBarColor(string hexColorStart, string hexColorEnd, bool isAnimated = false);

        void SetStatusBarColor(string hexColorStart);
    }
}
