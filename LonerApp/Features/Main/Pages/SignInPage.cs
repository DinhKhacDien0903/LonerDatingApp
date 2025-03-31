namespace LonerApp.Features.Pages;

public partial class SignInPage : BasePage
{
    private readonly MainPageModel _vm;
    public SignInPage(MainPageModel vm)
    {
        BindingContext = vm;
        _vm = vm;
        InitializeComponent();
    }
    protected override void OnAppearing()
    {
        _vm.IsBusy = true;
        base.OnAppearing();
        _vm.IsBusy = false;
    }

    public void Wellcome_SizeChanged(object sender, EventArgs e)
    {
        if (sender is Label label)
        {
            label.WidthRequest = Constants.WidthDevice * 0.8;
        }
    }
}