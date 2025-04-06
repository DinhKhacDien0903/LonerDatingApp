namespace LonerApp.Features.Pages;

public partial class SettingPage : BasePage
{
    private SettingPageModel _vm;
    public SettingPage(SettingPageModel vm)
	{
		BindingContext = _vm = vm;
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        _vm.IsBusy = true;
        base.OnAppearing();
        ServiceHelper.GetService<IDeviceService>().SetResizeKeyboardInput();
        _vm.IsBusy = false;
    }

    private void LocationEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not Editor LocationEditor)
            return;
    }

    private void Overlay_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is not BoxView boxview)
        {
            return;
        }

        LocationEditor.Unfocus();
        FromAgeEditor.Unfocus();
        ToAgeEditor.Unfocus();
        Overlay.IsVisible = false;
    }

    private void Overlay_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (sender is not BoxView boxview)
        {
            return;
        }

        if (e.StatusType == GestureStatus.Started)
        {
            LocationEditor.Unfocus();
            FromAgeEditor.Unfocus();
            ToAgeEditor.Unfocus();
            Overlay.IsVisible = false;
        }
    }

    private void LocationEditor_Unfocused(object sender, FocusEventArgs e)
    {
        if (sender is not Editor editor)
        {
            return;
        }

        ServiceHelper.GetService<IDeviceService>().HideKeyboard();
    }

    private void LocationEditor_Focused(object sender, FocusEventArgs e)
    {
        Overlay.IsVisible = true;
    }
}