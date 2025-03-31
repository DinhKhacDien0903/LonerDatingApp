namespace LonerApp.Features.Pages;

public partial class SetupUniversityPage : ContentPage
{
	public SetupUniversityPage(SetupPageModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}