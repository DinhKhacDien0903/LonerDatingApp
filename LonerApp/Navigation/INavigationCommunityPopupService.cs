using CommunityToolkit.Maui.Views;

namespace LonerApp.Navigation;

public interface INavigationCommunityPopupService
{
    Task<object> PushAsync<TViewModel>(object param = null)
        where TViewModel : BasePageModel;
    void PopPopup();
    Stack<Popup> GetPopupStack();
}