namespace LonerApp.Features.Pages;

public partial class EmailAuthor : BasePage
{
    private readonly LoginPageModel _vm;
    public EmailAuthor(LoginPageModel vm)
	{
		InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override void OnAppearing()
    {
        _vm.IsBusy = true;
        base.OnAppearing();
        _vm.IsBusy = false;
    }
}