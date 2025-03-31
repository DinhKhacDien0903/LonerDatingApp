namespace LonerApp.Features.Pages;

public partial class ImageViewerFullScreenPage : BasePage
{
    private ChatMessageImagePageModel _vm;
    public ImageViewerFullScreenPage(ChatMessageImagePageModel vm)
	{
        BindingContext = _vm = vm;
        InitializeComponent();
	}

    private void CachedImage_Loaded(object sender, EventArgs e)
    {
        if (sender is not FFImageLoading.Maui.CachedImage cachedImage)
        {
            return;
        }

        _vm.IsBusy = true;
        var x = Constants.WidthDevice;
        var y = Constants.HEIGHT_OF_SWIPE_CARD_VIEW_CONTAINER;
        
        cachedImage.HeightRequest = Constants.HEIGHT_OF_SWIPE_CARD_VIEW_CONTAINER;
        cachedImage.WidthRequest = Constants.WidthDevice - 20;
        _vm.IsBusy = false;
    }
}