namespace LonerApp.Features.Pages;

public partial class ChatPage : BasePage
{
    private ChatPageModel _vm;
    public ChatPage(ChatPageModel vm)
    {
        BindingContext = _vm = vm;
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        _vm.IsBusy = true;
        base.OnAppearing();
        if (!_vm.IsPushPageWithNavService && _vm.IsNeedLoadUsersData)
        {
            await _vm.InitDataAsync();
            await _vm.ViewIsAppearingAsync();
        }
        _vm.IsBusy = false;
    }

    private void AboutMeEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not Editor aboutMeEditor)
            return;
    }

    private void Overlay_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is not BoxView boxview)
        {
            return;
        }

        AboutMeEditor.Unfocus();
        // Overlay.IsVisible = false;
    }

    private void Overlay_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (sender is not BoxView boxview)
        {
            return;
        }

        if (e.StatusType == GestureStatus.Started)
        {
            AboutMeEditor.Unfocus();
            // Overlay.IsVisible = false;
        }
    }

    private void AboutMeEditor_Unfocused(object sender, FocusEventArgs e)
    {
        if (sender is not Editor editor)
        {
            return;
        }

        ServiceHelper.GetService<IDeviceService>().HideKeyboard();
    }

    private void AboutMeEditor_Focused(object sender, FocusEventArgs e)
    {
        // Overlay.IsVisible = true;
    }
}