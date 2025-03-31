namespace LonerApp.Features.Pages;

public partial class DetailProfilePage : BasePage
{
    private ProfilePageModel _vm;
	public DetailProfilePage(ProfilePageModel vm)
	{
        BindingContext = _vm = vm;
        InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        _vm.IsBusy = true;
        base.OnAppearing();
        if(!_vm.IsPushPageWithNavService)
        {
            await _vm.InitImages();
            await _vm.ViewIsAppearingAsync();
        }
        _vm.IsBusy = false;
    }
}