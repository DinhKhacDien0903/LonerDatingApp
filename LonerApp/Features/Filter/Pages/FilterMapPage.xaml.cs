using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Collections.ObjectModel;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace LonerApp.Features.Pages;

public partial class FilterMapPage : BasePage
{
    private FilterMapPageModel _vm;
    public FilterMapPage(FilterMapPageModel vm)
    {
        BindingContext = _vm = vm;
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    private async void Grid_Loaded(object sender, EventArgs e)
    {
        if (sender is not Grid grid)
            return;
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var location = await _vm.GetCurrentLocationAsync();
            mapLonerDatingApp.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(location.Latitude, location.Longitude), Distance.FromMiles(10)));
        });
    }

    private void Pin_MarkerClicked(object sender, PinClickedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await AlertHelper.ShowErrorAlertAsync("MarkerTapped" + (sender as Pin).BindingContext.ToString());
        });
    }

    private void Pin_InfoWindowClicked(object sender, PinClickedEventArgs e)
    {
        if (sender is not Pin pin)
            return;
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            _vm.IsBusy = true;
            await _vm.ShowProfileUserCommand.ExecuteAsync(pin.BindingContext);
            _vm.IsBusy = false;
        });
    }
}