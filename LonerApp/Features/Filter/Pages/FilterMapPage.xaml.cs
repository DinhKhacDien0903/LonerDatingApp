using Java.Sql;
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
    const string DISTRICT_PIN_NAME = "DistrictPin";
    private DistrictLocationModel? _currentDistrict;
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

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        //TODO fix bug when back from profile detail page
        //if (_vm.IsNeedLoadUsersData)
        //{
        //    await _vm.LoadDataAsync();
        //}
    }

    private void Grid_Loaded(object sender, EventArgs e)
    {
        if(_vm.IsNavigationInProgress)
            return;
        if (sender is not Grid grid)
            return;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            _vm.currentLocation = await _vm.GetCurrentLocationAsync();

            await Task.Delay(2000);
            mapLonerDatingApp.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(_vm.currentLocation.Latitude, _vm.currentLocation.Longitude), Distance.FromMiles(10)));
            await DrawCircleMap(mapLonerDatingApp, _vm.CurrentRadius, _vm.currentLocation);
        });
    }

    private async void Pin_MarkerClicked(object sender, PinClickedEventArgs e)
    {
        if(_vm.IsNavigationInProgress)
            return;
        if (sender is not Pin pin)
            return;
        //Location: in here
        await DrawPolyLine(mapLonerDatingApp, await _vm.GetCurrentLocationAsync(), pin.Location);
    }

    private void Pin_InfoWindowClicked(object sender, PinClickedEventArgs e)
    {
                if(_vm.IsNavigationInProgress)
            return;
        if (sender is not Pin pin)
            return;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            _vm.IsBusy = true;
            await _vm.ShowProfileUserCommand.ExecuteAsync(pin.BindingContext);
            _vm.IsBusy = false;
        });
    }

    private void DistrictFilterTapped(object sender, TappedEventArgs e)
    {
        _vm.IsVisibleDistrictCollection = !_vm.IsVisibleDistrictCollection;
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await _vm.LoadDistrictLocationAsync();
        });
    }

    private void RadiusFilterTapped(object sender, TappedEventArgs e)
    {
        _vm.IsShowRadiusSearchBar = !_vm.IsShowRadiusSearchBar;
        _vm.IsVisibleFilterContainer = !_vm.IsVisibleFilterContainer;
    }

    private async void RadiusSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (sender is not Slider slider)
            return;
        if (RadiusLabel == null || mapLonerDatingApp == null)
            return;

        double radius = e.NewValue;
        RadiusLabel.Text = string.Format("{0:F0} km", slider.Value);
        await DrawCircleMap(mapLonerDatingApp, radius, _vm.currentLocation);
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

    async Task AddDistrictPin(Map map, DistrictLocationModel district)
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            RemoveDistrictPin(map);
            var pin = new Pin
            {
                Label = district.ID,
                Address = district.Name,
                Type = PinType.Generic,
                Location = district.Location,
            };

            pin.AutomationId = DISTRICT_PIN_NAME;
            map.Pins.Add(pin);
        });
    }

    void RemoveDistrictPin(Map map)
    {
        var previousPin = map.Pins.FirstOrDefault(x => x.AutomationId == DISTRICT_PIN_NAME);
        if (previousPin != null)
            map.Pins.Remove(previousPin);
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
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            RemovePolyLine(mapLonerDatingApp);
            double radius = 10;
            if (RadiusSlider != null)
                radius = RadiusSlider.Value;
            mapLonerDatingApp.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(_vm.currentLocation.Latitude, _vm.currentLocation.Longitude), Distance.FromMiles(10)));
            await Task.Delay(2000);
            _vm.FilterRadiusSearchCommand.Execute(Math.Round(radius, 2));
            if (_currentDistrict != null)
                await AddDistrictPin(mapLonerDatingApp, _currentDistrict);
        });
    }

    private void ViewTransparentTapped(object sender, TappedEventArgs e)
    {
        if (sender is not Grid grid)
            return;

        _vm.IsVisibleDistrictCollection = false;
    }

    private void DistrictItem_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is not StackLayout stackLayout || stackLayout.BindingContext is not DistrictLocationModel district)
            return;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            _vm.IsBusy = true;
            _currentDistrict = district;
            _vm.IsVisibleDistrictCollection = false;
            RemovePolyLine(mapLonerDatingApp);
            mapLonerDatingApp.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(district.Location.Latitude, district.Location.Longitude), Distance.FromMiles(10)));
            await Task.Delay(2000);
            _vm.currentLocation = district.Location;
            await DrawCircleMap(mapLonerDatingApp, _vm.CurrentRadius, _vm.currentLocation);
            _vm.IsBusy = false;
            _vm.SelectDistrictCommand.Execute(district);
            await Task.Delay(200);
            await AddDistrictPin(mapLonerDatingApp, district);
        });
    }

    private void Reset_Tapped(object sender, TappedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            _vm.IsBusy = true;
            RemovePolyLine(mapLonerDatingApp);
            _vm.ResetData();
            await _vm.LoadDataAsync();
            await Task.Delay(1000);
            mapLonerDatingApp.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(_vm.currentLocation.Latitude, _vm.currentLocation.Longitude), Distance.FromMiles(10)));
            await DrawCircleMap(mapLonerDatingApp, _vm.CurrentRadius, _vm.currentLocation);
            _vm.IsBusy = false;
        });
    }
}