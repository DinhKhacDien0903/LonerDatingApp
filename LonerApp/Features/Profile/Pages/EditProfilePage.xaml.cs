using Microsoft.Maui.Controls.Shapes;

namespace LonerApp.Features.Pages;

public partial class EditProfilePage : BasePage
{
    private EditProfilePageModel _vm;
    public EditProfilePage(EditProfilePageModel vm)
    {
        BindingContext = _vm =vm;
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
        _vm.IsEditVisible = true;
        _vm.IsPreviewVisible = false;
    }

    private void Label_Preview_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is not Label label)
            return;

        label.TextColor = Color.FromArgb("#FE3675");
        EditLabel.TextColor = Color.FromArgb("#939393");
        _vm.IsEditVisible = false;
        _vm.IsPreviewVisible = true;
        _vm.ShowPreviewProfile();
    }

    private void CachedImage_Loaded(object sender, EventArgs e)
    {
        if (sender is not FFImageLoading.Maui.CachedImage cachedImage)
        {
            return;
        }

        cachedImage.Clip = new RoundRectangleGeometry
        {
            CornerRadius = 20,
            Rect = new Rect(0, 0, Constants.WidthDevice - 16, Constants.HEIGHT_OF_SWIPE_CARD_VIEW_CONTAINER - 20)
        };
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
        WorkEditor.Unfocus();
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
            WorkEditor.Unfocus();
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