namespace LonerApp.Features.Pages;

public partial class PhoneNumberAuthor : BasePage
{
    private readonly LoginPageModel _vm;
    public PhoneNumberAuthor(LoginPageModel vm)
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
}