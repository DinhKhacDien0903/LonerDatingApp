using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Collections.ObjectModel;
using static Android.Provider.MediaStore.Audio;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace LonerApp.Features.Pages;

public partial class FilterMapPage : BasePage
{
    const string POLYLINE_DISTANCE_NAME = "PolyLineDistance";
    const string SEARCH_RADIUS_CIRCLE_NAME = "SearchRadiusCircle";
    private Location _currentLocation = new();
    private FilterMapPageModel _vm;
    public FilterMapPage(FilterMapPageModel vm)
    {
        BindingContext = _vm = vm;
        InitializeComponent();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        _currentLocation = await _vm.GetCurrentLocationAsync();
    }

    private void Grid_Loaded(object sender, EventArgs e)
    {
        if (sender is not Grid grid)
            return;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            mapLonerDatingApp.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(_currentLocation.Latitude, _currentLocation.Longitude), Distance.FromMiles(10)));
        });
    }

    private async void Pin_MarkerClicked(object sender, PinClickedEventArgs e)
    {
        if (sender is not Pin pin)
            return;

        await DrawPolyLine(mapLonerDatingApp, _currentLocation, pin.Location);
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

    private void CategoryFilterTapped(object sender, TappedEventArgs e)
    {

    }

    private void RadiusFilterTapped(object sender, TappedEventArgs e)
    {
        //_vm.FilterRadiusPressedCommand.Execute(null);
        _vm.IsShowRadiusSearchBar = !_vm.IsShowRadiusSearchBar;
        _vm.IsVisibleFilterContainer = !_vm.IsVisibleFilterContainer;
    }

    private async void RadiusSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (sender is not Slider slider)
            return;

        double radius = e.NewValue;
        RadiusLabel.Text = string.Format("{0:F0} km", slider.Value);
        await DrawCircleMap(mapLonerDatingApp, radius, _currentLocation);
        //SearchRadiusCircle.Radius = Distance.FromMeters(radius);
        //SearchPlaces(currentLocation, radius, KeyWord);
    }

    async Task DrawPolyLine(Map map, Location currentLocation, Location targetLocation)
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            RemovePolyLine(map);
            var polyLine = new Polyline
            {
                StrokeColor = Color.FromArgb("#FE3675"),
                StrokeWidth = 8,
                Geopath = { currentLocation, targetLocation }
            };

            polyLine.AutomationId = POLYLINE_DISTANCE_NAME;
            map.MapElements.Add(polyLine);
        });
    }

    async Task DrawCircleMap(Map map, double radius, Location location)
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            RemoveCircleMap(map);

            Circle circle = new Circle
            {
                Center = location,
                Radius = Distance.FromKilometers(radius),
                FillColor = Color.FromArgb("#88f7d0dc"),
                StrokeColor = Color.FromArgb("#FE3675"),
                StrokeWidth = 4,
            };

            circle.AutomationId = SEARCH_RADIUS_CIRCLE_NAME;
            map.MapElements.Add(circle);
        });
    }

    void RemovePolyLine(Map map)
    {
        var previousPolyLine = map.MapElements.FirstOrDefault(x => x.AutomationId == POLYLINE_DISTANCE_NAME);
        if (previousPolyLine != null)
            map.MapElements.Remove(previousPolyLine);
    }

    void RemoveCircleMap(Map map)
    {
        var currentSearchRadiusCircle = map.MapElements.FirstOrDefault(x => x.AutomationId == SEARCH_RADIUS_CIRCLE_NAME);
        if (currentSearchRadiusCircle != null)
            map.MapElements.Remove(currentSearchRadiusCircle);
    }

    private void FilterRadiusSearch_Tapped(object sender, TappedEventArgs e)
    {
        RemovePolyLine(mapLonerDatingApp);
        double radius = 5;
        if (RadiusSlider != null)
            radius = RadiusSlider.Value;
        _vm.FilterRadiusSearchCommand.Execute(Math.Round(radius,2));
    }
}