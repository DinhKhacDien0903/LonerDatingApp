namespace LonerApp.Features.Pages;

public partial class NotificationPage : BasePage
{
	private readonly NotificationPageModel _vm;
	private bool _isFirstLoad = true;
	public NotificationPage(NotificationPageModel vm)
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

	protected override async void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);
		Shell.SetTabBarIsVisible(this, true);
		if (!_vm.IsPushPageWithNavService)
		{
			await _vm.LoadDataAsync();
		}
	}

    private void NotificationCollectionView_Loaded(object sender, EventArgs e)
    {
		if (sender is not CollectionView collectionView)
		{
			return;
		}

		_vm._notificationCollection = collectionView;

		if (_isFirstLoad)
		{
			Dispatcher.DispatchAsync(async () =>
			{
				_vm.IsBusy = true;
				await Task.Delay(150);

				if (_vm.Notifications.Any())
				{
					collectionView.ScrollTo(0, position: ScrollToPosition.Start, animate: false);
				}

				await Task.Delay(100);
				_isFirstLoad = false;
				_vm.IsBusy = false;
			});
		}
    }
}