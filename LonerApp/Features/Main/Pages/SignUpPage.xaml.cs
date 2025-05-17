namespace LonerApp.Features.Pages;

public partial class SignUpPage : BasePage
{
	public SignUpPage()
	{
        BindingContext = ServiceHelper.GetPageModelObservable<MainPageModel>();
        InitializeComponent();
	}

    public void Wellcome_SizeChanged(object sender, EventArgs e)
    {
        if (sender is Label label)
        {
            label.WidthRequest = Constants.WidthDevice * 0.8;
        }
    }
}