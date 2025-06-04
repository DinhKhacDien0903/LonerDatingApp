using CommunityToolkit.Mvvm.Input;
using LonerApp.Features.Filter.Services;
using Microsoft.Maui.Controls.Maps;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Globalization;

namespace LonerApp.PageModels
{
    public partial class FilterMapPageModel : BasePageModel
    {
        public bool IsNeedLoadUsersData = true;
        [ObservableProperty]
        private bool _isVisibleNavigation;
        [ObservableProperty]
        private bool _isVisibleFilterContainer;
        [ObservableProperty]
        private bool _isVisibleDistrictCollection;
        [ObservableProperty]
        private bool _isShowRadiusSearchBar;
        [ObservableProperty]
        private bool _hasBackButton;
        [ObservableProperty]
        private byte[]? _imageData;
        [ObservableProperty]
        private double _currentRadius = 10.0;
        [ObservableProperty]
        private ObservableCollection<UserPinModel> _pins = new();
        [ObservableProperty]
        private ObservableCollection<DistrictLocationModel> _districts = new();
        private ObservableCollection<DistrictLocationModel> _currentDistrinct;
        private static bool isFirstLoad = true;
        private IFilterService _filterService;
        public Location currentLocation;
        private string _currentUserId = string.Empty;
        ContentPage? _previousPage;
        SwipePageModel? _swipePageModel;
        private const int RETRY_TIME = 5;
        private static Location HANOI_LOCATION = new Location(21.0285, 105.8542);
        public FilterMapPageModel(
            INavigationService navigationService,
            IFilterService filterService)
            : base(navigationService, true)
        {
            IsVisibleNavigation = true;
            HasBackButton = true;
            _filterService = filterService;
        }

        public override async Task InitAsync(object? initData)
        {
            IsNeedLoadUsersData = false;
            _previousPage = AppShell.Current?.CurrentPage as ContentPage;
            if (_previousPage != null)
                _swipePageModel = _previousPage.BindingContext as SwipePageModel;
            if (initData is string userId)
                _currentUserId = userId;
            await base.InitAsync(initData);
        }

        public async Task HandleLikeAsync(object param)
        {
            try
            {
                IsBusy = true;
                if (_swipePageModel != null)
                {
                    await _swipePageModel.LikePressedCommand.ExecuteAsync(param);
                    await Task.Delay(200);
                    var request = new GetMemberByLocationAndRadiusRequest
                    {
                        UserId = _currentUserId,
                        Longitude = currentLocation.Longitude.ToString(CultureInfo.InvariantCulture),
                        Latitude = currentLocation.Latitude.ToString(CultureInfo.InvariantCulture),
                        Radius = CurrentRadius
                    };
                    Pins.Clear();
                    var pins = await LoadPinAsync(request);
                    Pins = new ObservableCollection<UserPinModel>(pins);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task HandleDisLikeAsync(object param)
        {
            try
            {
                IsBusy = true;
                if (_swipePageModel != null)
                {
                    await _swipePageModel.DislikePressedCommand.ExecuteAsync(param);
                    await Task.Delay(200);
                    var request = new GetMemberByLocationAndRadiusRequest
                    {
                        UserId = _currentUserId,
                        Longitude = currentLocation.Longitude.ToString(CultureInfo.InvariantCulture),
                        Latitude = currentLocation.Latitude.ToString(CultureInfo.InvariantCulture),
                        Radius = CurrentRadius
                    };
                    Pins.Clear();
                    var pins = await LoadPinAsync(request);
                    Pins = new ObservableCollection<UserPinModel>(pins);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override async Task LoadDataAsync()
        {
            await base.LoadDataAsync();
            IsBusy = true;
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                currentLocation = await GetCurrentLocationAsync();
                currentLocation.Longitude = Math.Round(currentLocation.Longitude, 2);
                currentLocation.Latitude = Math.Round(currentLocation.Latitude, 2);
                await Task.Delay(10);
            });
            var resultLocationUpdate = await _filterService.UpdateLocationAsync(
                new UpdateLocationRequest
                {
                    UserId = _currentUserId,
                    Latitude = currentLocation.Latitude.ToString(CultureInfo.InvariantCulture),
                    Longitude = currentLocation.Longitude.ToString(CultureInfo.InvariantCulture)
                });
            if (resultLocationUpdate != null && resultLocationUpdate.IsSuccess)
            {
                var request = new GetMemberByLocationAndRadiusRequest
                {
                    UserId = _currentUserId,
                    Longitude = currentLocation.Longitude.ToString(CultureInfo.InvariantCulture),
                    Latitude = currentLocation.Latitude.ToString(CultureInfo.InvariantCulture),
                    Radius = CurrentRadius
                };
                var pins = await LoadPinAsync(request);
                Pins = new ObservableCollection<UserPinModel>(pins);
                IsNeedLoadUsersData = true;
            }
            else
            {
                await AlertHelper.ShowErrorAlertAsync($"Lỗi khi cập nhật vị trí ", "Lỗi");
            }
            IsBusy = false;
            ShouldLoadData = false;
        }

        async Task CheckPermissionAsync()
        {
            var alert = await AlertHelper.ShowConfirmationAlertAsync(
                I18nHelper.Get("FilterMap_CheckPermission_TurnOnLocationInSetting"),
                I18nHelper.Get("Dealer_CheckPermission_AlertTitle"));
            if (alert)
                ServiceHelper.GetService<IOpenSetting>().OpenSettingScreen();
        }

        public async Task LoadDistrictLocationAsync()
        {
            IsBusy = true;
            ObservableCollection<DistrictLocationModel> districtLocation = new();
            try
            {
                if (_currentDistrinct != null && _currentDistrinct.Any())
                    return;
                using var stream = await FileSystem.OpenAppPackageFileAsync("DistrictLocation.json");
                using var reader = new StreamReader(stream);

                var locationJson = await reader.ReadToEndAsync();
                var cities = JsonConvert.DeserializeObject<ObservableCollection<DistrictLocationModel>>(locationJson);
                if (cities != null)
                {
                    foreach (var city in cities)
                    {
                        Districts.Add(city);
                    }
                }

                _currentDistrinct = new ObservableCollection<DistrictLocationModel>(Districts);
            }
            catch (Exception ex)
            {
                await AlertHelper.ShowErrorAlertAsync("Can't load district data");
                throw;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<Location> GetCurrentLocationAsync()
        {
            try
            {
                IsBusy = true;
                if (await CheckPermission.CheckPermissionAsync(Permission.Location) ==
                        PermisionResultKey.GoToSetting)
                {
                    await CheckPermissionAsync();
                }

                if (isFirstLoad)
                {
                    UserSetting.Remove(StorageKey.CurrentLocation.ToString());
                    isFirstLoad = false;
                }

                var currentLocation = UserSetting.GetObject<Location>(StorageKey.CurrentLocation) ?? null;
                if (currentLocation != null)
                    return currentLocation;

                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(40));
                var location = await Geolocation.GetLocationAsync(request);

                if (IsLocationValid(location))
                {
                    UserSetting.SetObject(StorageKey.CurrentLocation, location);
                    return location;
                }
            }
            catch (Exception ex)
            {
                await AlertHelper.ShowErrorAlertAsync(I18nHelper.Get("FilterMap_CheckPermission_TurnOnLocationInSetting"));
            }
            finally
            {
                IsBusy = false;
            }

            return new Location(21.0285, 105.8542);
        }

        private bool IsLocationValid(Location? location)
        {
            if (location == null)
                return false;
            if (location.Longitude < 0 || location.Latitude < 0)
                return false;
            if (location.Longitude > 180 || location.Latitude > 90)
                return false;
            return true;
        }

        [RelayCommand]
        async Task OnSearchPressedAsync(object param)
        {
            if (SearchPressedCommand.IsRunning || IsBusy)
                return;
            IsBusy = true;
            IsVisibleFilterContainer = !IsVisibleFilterContainer;
            IsShowRadiusSearchBar = false;
            IsVisibleDistrictCollection = false;
            await Task.Delay(100);
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnFilterRadiusSearchAsync(object param)
        {
            if (param is not double radius)
                return;
            if (FilterRadiusSearchCommand.IsRunning || IsBusy)
                return;

            IsBusy = true;
            IsShowRadiusSearchBar = !IsShowRadiusSearchBar;
            IsVisibleFilterContainer = !IsVisibleFilterContainer;
            IsVisibleDistrictCollection = false;
            //ObservableCollection<UserPinModel> result = GetPinsInRadius(_cachePins, radius, currentLocation);
            var request = new GetMemberByLocationAndRadiusRequest
            {
                UserId = _currentUserId,
                Longitude = currentLocation.Longitude.ToString(),
                Latitude = currentLocation.Latitude.ToString(),
                Radius = CurrentRadius
            };
            var pins = await LoadPinAsync(request);
            Pins.Clear();
            foreach (var pin in pins)
            {
                Pins.Add(pin);
                await Task.Delay(10);
            }
            await Task.Delay(100);
            IsBusy = false;
        }

        [RelayCommand]
        async Task OnSelectDistrictAsync(object param)
        {
            if (param is not DistrictLocationModel district)
                return;
            if (SelectDistrictCommand.IsRunning || IsBusy)
                return;

            IsBusy = true;
            if (Pins.Any())
                Pins.Clear();
            var request = new GetMemberByLocationAndRadiusRequest
            {
                UserId = _currentUserId,
                Longitude = district.Location.Longitude.ToString(),
                Latitude = district.Location.Latitude.ToString(),
                Radius = CurrentRadius
            };
            var pins = await LoadPinAsync(request);
            //Pins = FindUsersInCity(_cachePins, district.Location, CurrentRadius);
            foreach (var pin in pins)
            {
                Pins.Add(pin);
                await Task.Delay(10);
            }
            await Task.Delay(100);
            IsBusy = false;
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public async Task ResetData()
        {
            Pins.Clear();
            CurrentRadius = 10;
            var request = new GetMemberByLocationAndRadiusRequest
            {
                UserId = _currentUserId,
                Longitude = currentLocation.Longitude.ToString(),
                Latitude = currentLocation.Latitude.ToString(),
                Radius = CurrentRadius
            };
            var pins = await LoadPinAsync(request);
            foreach (var pin in pins)
            {
                Pins.Add(pin);
                await Task.Delay(10);
            }
            await Task.Delay(100);
        }

        private async Task<List<UserPinModel>> LoadPinAsync(GetMemberByLocationAndRadiusRequest request)
        {
            try
            {
                var data = (await _filterService.GetMemberByLocationAndRadiusAsync(request)).Users;
                List<UserPinModel> temp = [];
                foreach (var item in data)
                {
                    var pin = new UserPinModel
                    {
                        UserId = item.UserId,
                        ImageSource = item.AvatarUrl ?? "",
                        Label = item.UserName,
                        Address = item.Description,
                        Type = PinType.Place,
                        Location = new Location(double.Parse(item.Latitude, CultureInfo.InvariantCulture), double.Parse(item.Longitude, CultureInfo.InvariantCulture))
                    };

                    temp.Add(pin);
                }

                return temp;
            }
            catch (Exception e)
            {
                await AlertHelper.ShowErrorAlertAsync(e.Message, "Lỗi");
                return [];
            }
        }

        [RelayCommand]
        async Task OnBackAsync(object param)
        {
            if (!IsBusy)
            {
                IsBusy = true;
                await NavigationService.PopPageAsync();
                await Task.Delay(100);
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task OnShowProfileUserAsync(object param)
        {
            if (ShowProfileUserCommand.IsRunning)
                return;
            if (param is not UserPinModel user)
                return;

            await NavigationService.PushToPageAsync<DetailProfilePage>(param: user.UserId, isPushModal: true);
            await Task.Delay(100);
        }
    }
}