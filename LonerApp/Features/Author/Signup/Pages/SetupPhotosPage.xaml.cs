namespace LonerApp.Features.Pages;

public partial class SetupPhotosPage : ContentPage
{
	public SetupPhotosPage(SetupPageModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}