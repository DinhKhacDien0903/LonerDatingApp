namespace LonerApp.Features.Pages;

public partial class FilterMapPage : BasePage
{
	public FilterMapPage()
	{
		InitializeComponent();
	}

	protected async override void OnAppearing()
	{
        await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
    }
}