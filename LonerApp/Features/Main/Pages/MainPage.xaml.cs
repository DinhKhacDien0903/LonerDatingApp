namespace LonerApp.Features.Pages;

public partial class MainPage : BasePage
{
    private readonly MainPageModel _vm;
    public MainPage()
    {
        BindingContext = _vm = ServiceHelper.GetPageModelObservable<MainPageModel>();
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        _vm.IsBusy = true;
        base.OnAppearing();
        ServiceHelper.GetService<ISystemStyleManager>().SetStatusBarColor("#ffffff");
        _vm.IsBusy = false;
    }
    public void Welcome_SizeChanged(object sender, EventArgs e)
    {
        if (sender is Label label)
        {
            label.WidthRequest = Constants.WidthDevice * 0.8;
        }
    }

    private async void Signup_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SignInPage());
    }
}