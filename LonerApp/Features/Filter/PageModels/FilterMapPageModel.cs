using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls.Maps;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace LonerApp.PageModels
{
    public partial class FilterMapPageModel : BasePageModel
    {
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
        private UserPinModel? currentLocationPin;
        //TODO: Handel get data in server
        private static ObservableCollection<UserPinModel> _cachePins;
        public Location currentLocation;
        public FilterMapPageModel(INavigationService navigationService)
            : base(navigationService, true)
        {
            IsVisibleNavigation = true;
            HasBackButton = true;
        }

        public override async Task InitAsync(object? initData)
        {
            await base.InitAsync(initData);
        }

        public override async Task LoadDataAsync()
        {
            await base.LoadDataAsync();
            IsBusy = true;
            await MainThread.InvokeOnMainThreadAsync(async() =>
            {
                currentLocation = await GetCurrentLocationAsync();
                await Task.Delay(10);
            });
            await LoadPins();
            //_cachePins = new ObservableCollection<UserPinModel>(Pins);
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

        private async Task LoadPins()
        {
            _cachePins = new ObservableCollection<UserPinModel>
            {
                new UserPinModel
                {
                    ImageSource = "nnn.jpeg",
                    Label = "Nguyễn Văn A",
                    Address = "123 Đường ABC, Quận 1, TP.HCM",
                    Type = PinType.Place,
                    Location = new Location(10.7725, 106.6970)
                },
                new UserPinModel
                {
                    ImageSource = "nnn.jpeg",
                    Label = "Trần Thị B",
                    Address = "456 Đường XYZ, Quận 3, TP.HCM",
                    Type = PinType.Place,
                    Location = new Location(10.7818, 106.6826)
                },
                new UserPinModel
                {
                    ImageSource = "lllll.jpeg",
                    Label = "Lê Hoàng C",
                    Address = "789 Đường MNP, Quận 5, TP.HCM",
                    Type = PinType.Place,
                    Location = new Location(10.7589, 106.6718)
                },
                new UserPinModel
                {
                    ImageSource = "lllll.jpeg",
                    Label = "Phạm Thu D",
                    Address = "101 Đường QRS, Quận 7, TP.HCM",
                    Type = PinType.Place,
                    Location = new Location(10.7317, 106.7214)
                },
                new UserPinModel
                {
                    ImageSource = "lllll.jpeg",
                    Label = "Võ Minh E",
                    Address = "112 Đường TUV, Quận Bình Thạnh, TP.HCM",
                    Type = PinType.Place,
                    Location = new Location(10.8123, 106.7025)
                },
                new UserPinModel
                {
                    ImageSource = "lllll.jpeg",
                    Label = "Đặng Thị F",
                    Address = "131 Đường WXY, Quận Hải Châu, Đà Nẵng",
                    Type = PinType.Place,
                    Location = new Location(16.0678, 108.2207)
                },
                new UserPinModel
                {
                    ImageSource = "lllll.jpeg",
                    Label = "Hồ Văn G",
                    Address = "141 Đường ZAB, Quận Thanh Khê, Đà Nẵng",
                    Type = PinType.Place,
                    Location = new Location(16.0712, 108.1982)
                },
                new UserPinModel
                {
                    ImageSource = "mmm.jpeg",
                    Label = "Cao Thị H",
                    Address = "151 Đường CDE, Quận Sơn Trà, Đà Nẵng",
                    Type = PinType.Place,
                    Location = new Location(16.0889, 108.2435)
                },
                new UserPinModel
                {
                    ImageSource = "mmm.jpeg",
                    Label = "Bùi Thanh I",
                    Address = "161 Đường FGH, Quận Liên Chiểu, Đà Nẵng",
                    Type = PinType.Place,
                    Location = new Location(16.0967, 108.1752)
                },
                new UserPinModel
                {
                    ImageSource = "mmm.jpeg",
                    Label = "Đỗ Ngọc K",
                    Address = "171 Đường IJK, Quận Ngũ Hành Sơn, Đà Nẵng",
                    Type = PinType.Place,
                    Location = new Location(16.0385, 108.2514)
                },
                new UserPinModel
                {
                    ImageSource = "bbbb.jpeg",
                    Label = "Linh",
                    Address = "Dai hoc Bach Khoa Ha Noi",
                    Type = PinType.Place,
                    Location = new Location(21.0043, 105.8410)
                },
                new UserPinModel
                {
                    ImageSource = "bbbb.jpeg",
                    Label = "Trang",
                    Address = "Dai hoc Kinh Te Quoc Dan",
                    Type = PinType.Place,
                    Location = new Location(21.0026, 105.8460)
                },
                new UserPinModel
                {
                    Label = "Huong",
                    Address = "Dai hoc Ngoai Thuong Ha Noi",
                    Type = PinType.Place,
                    Location = new Location(21.0245, 105.8069)
                },
                new UserPinModel
                {
                    ImageSource = "bbbb.jpeg",
                    Label = "Thu",
                    Address = "Dai hoc Quoc Gia Ha Noi",
                    Type = PinType.Place,
                    Location = new Location(21.0371, 105.7835)
                },
                new UserPinModel
                {
                    ImageSource = "nnn.jpeg",
                    Label = "Nga",
                    Address = "Dai hoc Su Pham Ha Noi",
                    Type = PinType.SearchResult,
                    Location = new Location(21.0370, 105.7878)
                },
                new UserPinModel
                {
                    ImageSource = "bbbb.jpeg",
                    Label = "Mai",
                    Address = "Dai hoc Giao Thong Van Tai",
                    Type = PinType.SearchResult,
                    Location = new Location(21.0227, 105.8010)
                },
                new UserPinModel
                {
                    ImageSource = "nnn.jpeg",
                    Label = "Hanh",
                    Address = "Dai hoc Y Ha Noi",
                    Type = PinType.SearchResult,
                    Location = new Location(21.0029, 105.8327)
                },
                new UserPinModel
                {
                    ImageSource = "bbbb.jpeg",
                    Label = "Van",
                    Address = "Hoc vien Bao Chi va Tuyen Truyen",
                    Type = PinType.Place ,
                    Location = new Location(21.0284, 105.8028)
                },
                new UserPinModel
                {
                    ImageSource = "bbbb.jpeg",
                    Label = "Diep",
                    Address = "Dai hoc Xay Dung Ha Noi",
                    Type = PinType.Place,
                    Location = new Location(21.0015, 105.8502)
                },
                new UserPinModel
                {
                    ImageSource = "bbbb.jpeg",
                    Label = "Lan",
                    Address = "Dai hoc Thuong Mai Ha Noi",
                    Type = PinType.Place,
                    Location = new Location(21.0326, 105.7902)
                }
            };

            foreach (var pin in _cachePins)
            {
                pin.Address = $"{pin.Address} - {GetDistance(await GetCurrentLocationAsync(), pin.Location)} km";
            }
            var temp = GetPinsInRadius(_cachePins, CurrentRadius, currentLocation);
            //Pins = null;
            Pins = new ObservableCollection<UserPinModel>(temp);
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
        private double GetDistance(Location fromLocation, Location destinationLocation)
        {
            return Math.Round(Location.CalculateDistance(fromLocation, destinationLocation, DistanceUnits.Kilometers), 2);
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
                if (location != null)
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

        private ObservableCollection<UserPinModel> GetPinsInRadius(ObservableCollection<UserPinModel> allPins, double radius, Location curentLocation)
        {
            var result = new ObservableCollection<UserPinModel>();
            foreach (var pin in allPins)
            {
                double distance = Math.Round(Location.CalculateDistance(curentLocation, pin.Location, DistanceUnits.Kilometers), 2);
                if (distance <= radius)
                {
                    result.Add(pin);
                }
            }

            return result;
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
            //await NavigationService.PushToPageAsync<SearchPage>(isPushModal: true);
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
            ObservableCollection<UserPinModel> result = GetPinsInRadius(_cachePins, radius, currentLocation);
            Pins.Clear();
            foreach (var pin in result)
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
            Pins = FindUsersInCity(_cachePins, district.Location, CurrentRadius);
            await Task.Delay(100);
            IsBusy = false;
        }

        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return 6371 * c;
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public void ResetData()
        {
            _cachePins = null;
            Pins.Clear();
        }

        public ObservableCollection<UserPinModel> FindUsersInCity(ObservableCollection<UserPinModel> userPins, Location district, double radius)
        {
            var usersPinInCity = new ObservableCollection<UserPinModel>();
            foreach (var user in userPins)
            {
                var distance = CalculateDistance(district.Latitude, district.Longitude, user.Location.Latitude, user.Location.Longitude);
                if (distance <= radius)
                {
                    usersPinInCity.Add(user);
                }
            }

            return usersPinInCity;
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
            {
                return;
            }
            //Todo: get userId for detailProfile
            await NavigationService.PushToPageAsync<DetailProfilePage>(isPushModal: true);
            await Task.Delay(100);
        }
    }
}