namespace LonerApp.Features.Pages;

using LonerApp.Features.Services;
using Microsoft.Maui.Controls.Shapes;
using Plugin.Maui.SwipeCardView.Core;

public partial class MainSwipePage : BasePage
{
    private readonly SwipePageModel _vm;
    private readonly ISwipeService _swipeService;
    public MainSwipePage(SwipePageModel vm, ISwipeService swipeService)
    {
        BindingContext = _vm = vm;
        _swipeService = swipeService;
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        _vm.IsBusy = true;
        base.OnAppearing();
        _vm.IsBusy = false;
    }

    private void SwipeCardViewContainer_Loaded(object sender, EventArgs e)
    {
        if (sender is not Grid swipeCardViewContainer)
        {
            return;
        }

        swipeCardViewContainer.HeightRequest = Constants.HEIGHT_OF_SWIPE_CARD_VIEW_CONTAINER;
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

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        if (!_vm.IsPushPageWithNavService && _vm.IsNeedLoadUsersData)
        {
           await _vm.LoadDataAsync();
        }
    }

    private void SwipeCardView_Swiped(object sender, Plugin.Maui.SwipeCardView.Core.SwipedCardEventArgs e)
    {
        if (e.Direction == Plugin.Maui.SwipeCardView.Core.SwipeCardDirection.Right)
        {
            _vm.LikePressedCommand.Execute(e.Item);
        }
        else if (e.Direction == Plugin.Maui.SwipeCardView.Core.SwipeCardDirection.Up)
        {
            _vm.StarPressedCommand.Execute(e.Item);
        }
        else if (e.Direction == Plugin.Maui.SwipeCardView.Core.SwipeCardDirection.Left)
        {
            _vm.DislikePressedCommand.Execute(e.Item);
        }
        else if (e.Direction == Plugin.Maui.SwipeCardView.Core.SwipeCardDirection.Down)
        {
            _vm.OpenDetailProfileCommand.Execute(e.Item);
        }
    }

    private void OnDislikeClicked(object sender, EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        SwipeCardView.InvokeSwipe(SwipeCardDirection.Left);
        _vm.LikePressedCommand.Execute(button.BindingContext);
    }

    private void OnSuperLikeClicked(object sender, EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        SwipeCardView.InvokeSwipe(SwipeCardDirection.Up);
        _vm.StarPressedCommand.Execute(button.BindingContext);
    }

    private void OnLikeClicked(object sender, EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        SwipeCardView.InvokeSwipe(SwipeCardDirection.Right);
        _vm.LikePressedCommand.Execute(button.BindingContext);
    }
}