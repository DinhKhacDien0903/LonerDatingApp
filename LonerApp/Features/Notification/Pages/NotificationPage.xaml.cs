namespace LonerApp.Features.Pages;

public partial class NotificationPage : BasePage
{
	private readonly NotificationPageModel _vm;
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
}