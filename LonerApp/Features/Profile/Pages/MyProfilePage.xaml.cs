
namespace LonerApp.Features.Pages;

public partial class MyProfilePage : BasePage
{
	private MyProfilePageModel _vm;
	public MyProfilePage(MyProfilePageModel vm)
	{
		BindingContext = _vm = vm;
		InitializeComponent();
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
		await _vm.LoadDataAsync();
        base.OnNavigatedTo(args);
    }
}