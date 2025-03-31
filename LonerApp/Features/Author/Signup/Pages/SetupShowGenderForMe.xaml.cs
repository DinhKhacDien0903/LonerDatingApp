namespace LonerApp.Features.Pages;

public partial class SetupShowGenderForMe : ContentPage
{
	public SetupShowGenderForMe(SetupPageModel vm)
	{
        BindingContext = vm;
        InitializeComponent();
	}
}