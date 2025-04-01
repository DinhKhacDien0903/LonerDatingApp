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
        if (sender is not Pin pin)
            return;
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var currentLocation = await _vm.GetCurrentLocationAsync();
            var targetLocation = pin.Location;

            mapLonerDatingApp.MapElements.Clear();

            var polyLine = new Polyline
            {
                StrokeColor = Colors.Pink,
                StrokeWidth = 10,
                Geopath = { currentLocation, targetLocation }
            };

            var currentLabel = pin.Label;
            var distance = Location.CalculateDistance(currentLocation, targetLocation, DistanceUnits.Kilometers);

            //TODO: change Km for current
            var model = pin.BindingContext as UserPinModel;
            model.Label = $"{currentLabel} - {distance:F2} km";
            mapLonerDatingApp.MapElements.Add(polyLine);
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