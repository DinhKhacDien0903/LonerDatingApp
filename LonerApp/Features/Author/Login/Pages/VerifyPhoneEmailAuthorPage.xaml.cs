namespace LonerApp.Features.Pages;

public partial class VerifyPhoneEmailAuthorPage : BasePage
{
	public VerifyPhoneEmailAuthorPage()
	{
		BindingContext = ServiceHelper.GetPageModelObservable<VerfyEmailPageModel>();
		InitializeComponent();
	}
}