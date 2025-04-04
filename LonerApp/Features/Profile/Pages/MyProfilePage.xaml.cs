namespace LonerApp.Features.Pages;

public partial class MyProfilePage : BasePage
{
	public MyProfilePage(MyProfilePageModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}