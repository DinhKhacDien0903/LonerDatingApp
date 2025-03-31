namespace LonerApp.Features.Pages;

public partial class VerifyPhoneNumberAuthorPage : BasePage
{
    private readonly LoginPageModel _vm;
    public VerifyPhoneNumberAuthorPage(LoginPageModel vm)
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