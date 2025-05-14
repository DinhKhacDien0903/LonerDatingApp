using CommunityToolkit.Maui.Views;

namespace LonerApp.UI.Controls;

public class CustomCommunityPopup : Popup
{
    public delegate void BackButtonEventHandler(object sender, EventArgs e);
    public delegate void ViewApearhandler(object sender, EventArgs e);
    public delegate void ViewDisappearHandler(object sender, EventArgs e);
    public delegate void ViewApearedHandler(object sender, EventArgs e);

    public event BackButtonEventHandler OnBackButton;
    public event ViewApearhandler OnViewApear;
    public event ViewDisappearHandler OnViewDisappear;
    public event ViewApearedHandler OnApeared;

    public void OnBackButtonPressed()
    {
        OnBackButton?.Invoke(this, null);
    }

    public void OnViewApearing()
    {
        OnViewApear?.Invoke(this, null);
    }

    public void OnViewDisappearing()
    {
        OnViewDisappear?.Invoke(this, null);
    }

    public void OnViewApeared()
    {
        OnApeared?.Invoke(this, null);
    }

    public virtual void OnResume()
    {
    }

    public virtual void OnSleep()
    {
    }
}