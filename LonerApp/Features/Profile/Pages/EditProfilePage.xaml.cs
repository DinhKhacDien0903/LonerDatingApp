using Google.Android.Material.Color.Utilities;

namespace LonerApp.Features.Pages;

public partial class EditProfilePage : BasePage
{
    public EditProfilePage(EditProfilePageModel vm)
    {
        BindingContext = vm;
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        var x = 10;
        base.OnAppearing();
    }

    private void Label_Edit_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is not Label label)
            return;

        label.TextColor = Color.FromArgb("#FE3675");
        PreviewLabel.TextColor = Color.FromArgb("#939393");
    }

    private void Label_Preview_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is not Label label)
            return;

        label.TextColor = Color.FromArgb("#FE3675");
        EditLabel.TextColor = Color.FromArgb("#939393");
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

        var point = e.GetPosition(InputGrid);
        AboutMeEditor.Unfocus();
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
            AboutMeEditor.Unfocus();
            Overlay.IsVisible = false;
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
        Overlay.IsVisible = true;
    }
}