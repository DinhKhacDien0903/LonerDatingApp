namespace LonerApp.Features.Pages;

public partial class SetupInterestPage : ContentPage
{
	public SetupInterestPage(SetupPageModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}