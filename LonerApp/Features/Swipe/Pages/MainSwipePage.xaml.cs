namespace LonerApp.Features.Pages;

using Microsoft.Maui.Controls.Shapes;
using Plugin.Maui.SwipeCardView;
using Plugin.Maui.SwipeCardView.Core;

public partial class MainSwipePage : BasePage
{
    private bool _isSwiped;
    private readonly SwipePageModel _vm;
    public MainSwipePage(SwipePageModel vm)
    {
        BindingContext = _vm = vm;
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
        Shell.SetTabBarIsVisible(this, true);
        if (!_vm.IsPushPageWithNavService && _vm.IsNeedLoadUsersData)
        {
           await _vm.LoadDataAsync();
        }
    }

    private void OnDislikeClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || _isSwiped)
        {
            return;
        }

        SwipeCardView.InvokeSwipe(SwipeCardDirection.Left);
        _vm.LikePressedCommand.Execute(button.BindingContext);
        _isSwiped = true;
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
        if (sender is not Button button || _isSwiped)
        {
            return;
        }

        SwipeCardView.InvokeSwipe(SwipeCardDirection.Right);
        _vm.LikePressedCommand.Execute(button.BindingContext);
        _isSwiped = true;
    }

    private SwipeCardDirection? _lastSwipeDirection;
    private void SwipeCardView_Swiped(object sender, SwipedCardEventArgs e)
    {
        if (e.Direction == SwipeCardDirection.Right && !_isSwiped)
        {
            _lastSwipeDirection = e.Direction;
            _vm.LikePressedCommand.Execute(e.Item);
        }
        else if (e.Direction == SwipeCardDirection.Up)
        {
            _vm.StarPressedCommand.Execute(e.Item);
        }
        else if (e.Direction == SwipeCardDirection.Left && !_isSwiped)
        {
            _lastSwipeDirection = e.Direction;
            _vm.DislikePressedCommand.Execute(e.Item);
        }
        else if (e.Direction == SwipeCardDirection.Down)
        {
            _vm.OpenDetailProfileCommand.Execute(e.Item);
        }

        //reset
        _isSwiped = false;
    }

    private void SwipeCardView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if(sender is not SwipeCardView swipeCardView)
            return;

        if(e.PropertyName != nameof(SwipeCardView.TopItem))
            return;

        _vm.OnTopItemPropertyChanged(swipeCardView.TopItem);
    }
}