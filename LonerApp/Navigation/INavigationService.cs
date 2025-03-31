namespace LonerApp.Navigation
{
    public interface INavigationService
    {
        /// <summary>
        /// Navigation to any Page in the application.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param">Address Page need redirect</param>
        /// <param name="isPushModal">Push to Modal?, default is false</param>
        /// <param name="isAnimation">Animation when navigation</param>
        /// <returns></returns>
        Task PushToPageAsync<T>(object? param = null, bool isPushModal = false, bool isAnimation = true)
            where T : ContentPage;

        Task PopPageAsync(bool isPopModal = false, bool isAnimation = true);

        Task PopToPageInStackAsync<T>(Type targetPageType, bool isPopModal = false, bool isAnimation = true);

        Task PopToRootAsync(bool isAnimation = true);

        int GetNumberStack();

        Task PushToWebViewAsync<T>(object param) where T : ContentPage;
    }
}
