using Mopups.Pages;

namespace LonerApp.Navigation
{
    public interface IPopupNavigationService
    {
        Task PushAsync<T>(object param = null, bool animate = true)
    where T : PopupPage;
        Task PopAsync(bool animate = true);
        Task PopAllAsync(bool animate = true);
    }
}