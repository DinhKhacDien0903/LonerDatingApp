namespace LonerApp.Features.Pages;

public partial class EmailAuthor : BasePage
{
    private readonly LoginPageModel _vm;
    public EmailAuthor()
	{
		InitializeComponent();
        BindingContext = _vm =  ServiceHelper.GetPageModelObservable<LoginPageModel>();
    }

    protected override void OnAppearing()
    {
        _vm.IsBusy = true;
        base.OnAppearing();
        _vm.IsBusy = false;
    }
}