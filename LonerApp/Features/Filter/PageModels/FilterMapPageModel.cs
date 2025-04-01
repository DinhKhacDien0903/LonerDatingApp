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
        private bool _hasBackButton;
        [ObservableProperty]
        private byte[]? _imageData;
        [ObservableProperty]
        private ObservableCollection<UserPinModel> _pins = new();
        private static bool isFirstLoad = true;

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
            ShouldLoadData = false;
            IsBusy = true;
            if (await CheckPermission.CheckPermissionAsync(Permission.Location) ==
                PermisionResultKey.GoToSetting)
            {
                await CheckPermissionAsync();
            }
            LoadPins();
            IsBusy = false;
            ShouldLoadData = true;
        }

        async Task CheckPermissionAsync()
        {
            var alert = await AlertHelper.ShowConfirmationAlertAsync(
                I18nHelper.Get("FilterMap_CheckPermission_TurnOnLocationInSetting"),
                I18nHelper.Get("Dealer_CheckPermission_AlertTitle"));
            if (alert)
                ServiceHelper.GetService<IOpenSetting>().OpenSettingScreen();
        }

        private void LoadPins()
        {
            Pins = new ObservableCollection<UserPinModel>
            {
                new UserPinModel
                {  
                    Label = "Linh",
                    Address = "Dai hoc Bach Khoa Ha Noi",
                    Type = PinType.Place,
                    Location = new Location(21.0043, 105.8410)
                },
                new UserPinModel
                {
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
                    Label = "Thu",
                    Address = "Dai hoc Quoc Gia Ha Noi",
                    Type = PinType.Place,
                    Location = new Location(21.0371, 105.7835)
                },
                new UserPinModel
                {
                    Label = "Nga",
                    Address = "Dai hoc Su Pham Ha Noi",
                    Type = PinType.SearchResult,
                    Location = new Location(21.0370, 105.7878)
                },
                new UserPinModel
                {
                    Label = "Mai",
                    Address = "Dai hoc Giao Thong Van Tai",
                    Type = PinType.SearchResult,
                    Location = new Location(21.0227, 105.8010)
                },
                new UserPinModel
                {
                    Label = "Hanh",
                    Address = "Dai hoc Y Ha Noi",
                    Type = PinType.SearchResult,
                    Location = new Location(21.0029, 105.8327)
                },
                new UserPinModel
                {
                    Label = "Van",
                    Address = "Hoc vien Bao Chi va Tuyen Truyen",
                    Type = PinType.Place ,
                    Location = new Location(21.0284, 105.8028)
                },
                new UserPinModel
                {
                    Label = "Diep",
                    Address = "Dai hoc Xay Dung Ha Noi",
                    Type = PinType.Place,
                    Location = new Location(21.0015, 105.8502)
                },
                new UserPinModel
                {
                    Label = "Lan",
                    Address = "Dai hoc Thuong Mai Ha Noi",
                    Type = PinType.Place,
                    Location = new Location(21.0326, 105.7902)
                }
            };
        }

        public async Task<Location> GetCurrentLocationAsync()
        {
            try
            {
                IsBusy = true;
                //if(isFirstLoad)
                //{
                //    UserSetting.Remove(StorageKey.CurrentLocation.ToString());
                //    isFirstLoad = false;
                //}

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

            return null;
        }

        [RelayCommand]
        async Task OnSearchPressedAsync(object param)
        {
            if (!IsBusy)
            {
                IsBusy = true;
                //await NavigationService.PushToPageAsync<SearchPage>(isPushModal: true);
                await Task.Delay(100);
                IsBusy = false;
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