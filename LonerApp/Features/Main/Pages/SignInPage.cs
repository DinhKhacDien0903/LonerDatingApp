namespace LonerApp.Features.Pages;

public partial class SignInPage : BasePage
{
    public SignInPage()
    {
        InitializeComponent();
        BindingContext = ServiceHelper.GetPageModelObservable<MainPageModel>();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    public void Wellcome_SizeChanged(object sender, EventArgs e)
    {
        if (sender is Label label)
        {
            label.WidthRequest = Constants.WidthDevice * 0.8;
        }
    }

    private async void Google_SignIn_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VerifyPhoneEmailAuthorPage());
    }

    private async void PhoneNumber_SignIn_Tapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PhoneNumberAuthor());
    }
}