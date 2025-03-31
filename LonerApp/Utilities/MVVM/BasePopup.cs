using CommunityToolkit.Maui.Views;

namespace LonerApp.Utilities.MVVM
{
    public class BasePopup : Popup
    {
        public BasePopup()
        {
            Shell.SetPresentationMode(this, PresentationMode.Modal);
        }
    }
}
