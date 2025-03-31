namespace LonerApp.Features.Pages;

public partial class SetupGenderPage : ContentPage
{
	public SetupGenderPage(SetupPageModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}