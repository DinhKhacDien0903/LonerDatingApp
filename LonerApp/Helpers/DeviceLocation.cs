namespace LonerApp.Helpers;
 public class DeviceLocation
    {
        private const int RetryTime = 5;
        private const int MinAccuracy = 500;
        private static Location _lastKnowLocation;
        private static readonly IDeviceService _deviceService = ServiceHelper.GetService<IDeviceService>();
        private static readonly IFusedLocationService _fusedLocationService = ServiceHelper.GetService<IFusedLocationService>();
        public static async Task RefreshDeviceLocationAsync()
        {
            try
            {
                var statusPermission = await GetLocationPermissionStatusAsync();
                if (statusPermission == PermissionStatus.Granted || statusPermission == PermissionStatus.Restricted)
                {
                    // bool highAccuracyMode = true;
                    Location deviceLocation = null;
#if ANDROID
                    // highAccuracyMode = _deviceService.IsHighAccuracyEnabled();
                    deviceLocation = await _fusedLocationService.RequestLocationUpdatesAsync();
#elif IOS
                    deviceLocation = await Geolocation.Default.GetLastKnownLocationAsync();
#endif
                    if (NeedsLocationUpdate(statusPermission, deviceLocation) && true)
                    {
                        deviceLocation = await RequestNewLocationAsync();
                    }

                    if (deviceLocation != null)
                    {
                        _lastKnowLocation = deviceLocation;
                    }
                }
            }
#if DEBUG
            catch (FeatureNotSupportedException fnsEx)
            {
                System.Console.WriteLine("Not supported on device: " + fnsEx.Message);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                System.Console.WriteLine("Not enabled on device: " + fneEx.Message);
            }
            catch (PermissionException pEx)
            {
                System.Console.WriteLine("Permission exception: " + pEx.Message);
            }
#endif
            catch (Exception)
            {
            }
        }

        private static async Task<Location> RequestNewLocationAsync()
        {
            for (int i = 1; i <= RetryTime; i++)
            {
                try
                {
                    return await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(5)));
                }
                catch (Exception)
                {
                    if (i == RetryTime)
                    {
                        throw;
                    }
                }
            }

            return null;
        }

        private static bool NeedsLocationUpdate(PermissionStatus status, Location location)
        {
            return status != PermissionStatus.Restricted && (location == null || location.Accuracy > MinAccuracy);
        }

        private static async Task<PermissionStatus> GetLocationPermissionStatusAsync()
        {
            // ToDo: https://github.com/dotnet/maui/issues/23060
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
#if ANDROID
            if (status == PermissionStatus.Denied)
                status = _deviceService.GetNativePermissionStatus(Permission.Location);
#endif
            return status;
        }

        public static async Task<Location> GetDeviceLocationAsync()
        {
            if (_lastKnowLocation == null)
                await RefreshDeviceLocationAsync();
            return _lastKnowLocation;
        }
    }