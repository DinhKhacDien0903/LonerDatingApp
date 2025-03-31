namespace LonerApp.Helpers;
public static class Constants
{
    public static double HEIGHT_OF_NAVIGATION_BAR = 56;
    public static double WidthDevice { get; set; } = DeviceDisplay.Current.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
    public static double HeightDevice { get; set; } = DeviceDisplay.Current.MainDisplayInfo.Height / DeviceDisplay.Current.MainDisplayInfo.Density;
    public static double HeightOfTabBar { get; set; } = NavigationPage.GetHasNavigationBar(AppHelper.CurrentMainPage) ? 50 : 0;

    public static double HEIGHT_OF_SWIPE_CARD_VIEW_CONTAINER = HeightDevice - HeightOfTabBar - HEIGHT_OF_NAVIGATION_BAR - 100;
}