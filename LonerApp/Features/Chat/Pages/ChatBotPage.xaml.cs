namespace LonerApp.Features.Pages;

public partial class ChatBotPage : BasePage
{
    private readonly ChatBotPageModel _vm;
    private bool _isFirstLoad = true;
    public ChatBotPage(ChatBotPageModel vm)
	{
        BindingContext = _vm = vm;
		InitializeComponent();
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
        if (sender is not Editor editor)
        {
            return;
        }
    }

    private void ChatList_Loaded(object sender, EventArgs e)
    {
        if (sender is not CollectionView collectionView)
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
        if (sender is not Editor editor)
        {
            return;
        }

        Overlay.IsVisible = true;
        string hexColorChange = "#f7b2c8";
        InputGrid.Stroke = Color.FromArgb(hexColorChange);
        lbSend.TextColor = Color.FromArgb(hexColorChange);
        MessageEditor.TextColor = Color.FromArgb(hexColorChange);
    }

    private void MessageEditor_Unfocused(object sender, FocusEventArgs e)
    {
        InputGrid.Stroke = Color.FromArgb("#D9D9D9");
        lbSend.TextColor = Color.FromArgb("#939393");
        MessageEditor.TextColor = Color.FromArgb("#939393");
        ServiceHelper.GetService<IDeviceService>().HideKeyboard();
    }

    private void Overlay_Tapped(object sender, TappedEventArgs e)
    {
        var point = e.GetPosition(InputGrid);
        if (!ChatMessageList.Bounds.Contains(point.Value))
        {
            MessageEditor.Unfocus();
            Overlay.IsVisible = false;
            _vm.IsVisibleOverlay = false;
            _vm.IsVisibleOption = false;
        }
    }

    private void Overlay_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (e.StatusType == GestureStatus.Started)
        {
            MessageEditor.Unfocus();
            Overlay.IsVisible = false;
            _vm.IsVisibleOverlay = false;
            _vm.IsVisibleOption = false;
        }
    }
}