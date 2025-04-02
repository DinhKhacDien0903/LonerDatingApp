using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls.Maps;
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
        private bool _isShowRadiusSearchBar;
        [ObservableProperty]
        private bool _hasBackButton;
        [ObservableProperty]
        private byte[]? _imageData;
        [ObservableProperty]
        private ObservableCollection<UserPinModel> _pins = new();
        private static bool isFirstLoad = true;
        private UserPinModel? currentLocationPin;
        //TODO: Handel get data in server
        private static ObservableCollection<UserPinModel> _cachePins;
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
            await LoadPins();
            _cachePins = new ObservableCollection<UserPinModel>(Pins);
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
            Pins = new ObservableCollection<UserPinModel>
            {
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
                    ImageSource = "bbbb.jpeg",
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
                    ImageSource = "bbbb.jpeg",
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

            foreach(var pin in Pins)
            {
                pin.Address = $"{pin.Address} - {GetDistance(await GetCurrentLocationAsync(), pin.Location)} km";
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

                if(isFirstLoad)
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

            return new Location(21.0285,105.8542);
        }

        private ObservableCollection<UserPinModel> GetPinsInRadius(ObservableCollection<UserPinModel> allPins, double radius, Location curentLocation)
        {
            var result = new ObservableCollection<UserPinModel>();
            foreach (var pin in allPins)
            {
                double distance = Math.Round(Location.CalculateDistance(curentLocation, pin.Location, DistanceUnits.Kilometers),2);
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
            ObservableCollection<UserPinModel> result = GetPinsInRadius(_cachePins, radius, await GetCurrentLocationAsync());
            Pins.Clear();
            Pins = result;
            await Task.Delay(100);
            IsBusy = false;
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
            if(ShowProfileUserCommand.IsRunning)
            {
                return;
            }
            //Todo: get userId for detailProfile
            await NavigationService.PushToPageAsync<DetailProfilePage>(isPushModal: true);
            await Task.Delay(100);
        }
    }
}