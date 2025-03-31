namespace LonerApp.Features.Pages;

public partial class SetupNamePage : ContentPage
{
	public SetupNamePage(SetupPageModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}