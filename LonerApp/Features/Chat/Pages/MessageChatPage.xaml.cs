namespace LonerApp.Features.Pages;

public partial class MessageChatPage : BasePage
{
    private ChatMessagePageModel _vm;
    private bool _isFirstLoad = true;
    public MessageChatPage(ChatMessagePageModel vm)
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

    private async void OnScrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        if (sender is not CollectionView collectionView)
        {
            return;
        }

        if (e.VerticalDelta < 0 && e.FirstVisibleItemIndex <= 2)
        {
            await _vm.LoadMoreMessagesCommand.ExecuteAsync(null);
        }
    }

    private void MessageEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if(sender is not Editor editor)
        {
            return;
        }
    }

    private void ChatList_Loaded(object sender, EventArgs e)
    {
        if(sender is not CollectionView collectionView)
        {
            return;
        }

        _vm.GetChatListCollectionView();

        if (_isFirstLoad)
        {
            Dispatcher.DispatchAsync(async () =>
            {
                _vm.IsBusy = true;
                await Task.Delay(150);

                if (_vm.Messages.Any())
                {
                    collectionView.ScrollTo(_vm.Messages.Last(), position: ScrollToPosition.End, animate: false);
                }

                _isFirstLoad = false;
                _vm.IsBusy = false;
            });
        }
    }

    private void MessageEditor_Focused(object sender, FocusEventArgs e)
    {
        if(sender is not Editor editor)
        {
            return;
        }

        lbUploadImage.IsVisible = false;
    }

    private void MessageEditor_Unfocused(object sender, FocusEventArgs e)
    {
        if (sender is not Editor editor)
        {
            return;
        }

        lbUploadImage.IsVisible = true;
    }
}