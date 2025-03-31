namespace LonerApp.Features.Pages;

public partial class SetupDateOfBirthPage : ContentPage
{
	public SetupDateOfBirthPage(SetupPageModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}