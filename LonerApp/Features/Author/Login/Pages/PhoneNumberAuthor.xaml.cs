namespace LonerApp.Features.Pages;

public partial class PhoneNumberAuthor : BasePage
{
    private readonly LoginPageModel _vm;
    public PhoneNumberAuthor()
    {
        BindingContext = _vm = ServiceHelper.GetPageModelObservable<LoginPageModel>();
        InitializeComponent();
    }
    protected override void OnAppearing()
    {
        _vm.IsBusy = true;
        base.OnAppearing();
        _vm.IsBusy = false;
    }
}