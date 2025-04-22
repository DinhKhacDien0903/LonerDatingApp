namespace LonerApp.Features.Pages;

public partial class VerifyPhoneEmailAuthorPage : BasePage
{
	public VerifyPhoneEmailAuthorPage(VerfyEmailPageModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}