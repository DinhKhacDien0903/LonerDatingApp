using Android.App;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Tasks;

namespace LonerApp.Services;
public class FusedLocationService : IFusedLocationService
{
    private const long ONE_MINUTE = 60 * 1000;
    private readonly IFusedLocationProviderClient _fusedLocationClient = LocationServices.GetFusedLocationProviderClient(
        global::Android.App.Application.Context);

    public async Task<Location> RequestLocationUpdatesAsync()
    {
        try
        {
            var resultAccuracy = await CheckLocationAccuracySettingsAsync();
            if (!resultAccuracy)
            {
                return null;
            }

            return await RequestSingleLocationUpdateAsync();
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static Location ToLocationModel(Android.Locations.Location location)
    {
        return new Location
        {
            Longitude = location.Longitude,
            Latitude = location.Latitude,
            Accuracy = location.Accuracy
        };
    }

    private async Task<bool> CheckLocationAccuracySettingsAsync()
    {
        try
        {
            var locationRequest = CreateLocationRequest();
            var settingsRequest = CreateSettingsRequest(locationRequest);
            return await CheckLocationSettingsAsync(settingsRequest);
        }
        catch
        {
            return false;
        }
    }

    private static LocationRequest CreateLocationRequest()
    {
        return new LocationRequest.Builder(Priority.PriorityHighAccuracy)
            .SetIntervalMillis(ONE_MINUTE)
            .Build();
    }

    private static LocationSettingsRequest CreateSettingsRequest(LocationRequest locationRequest)
    {
        return new LocationSettingsRequest.Builder()
            .AddLocationRequest(locationRequest)
            .SetAlwaysShow(true)
            .Build();
    }

    private async Task<bool> CheckLocationSettingsAsync(LocationSettingsRequest settingsRequest)
    {
        if (Platform.CurrentActivity != null)
        {
            var tcs = new TaskCompletionSource<bool>();
            var client = LocationServices.GetSettingsClient(Platform.CurrentActivity);
            var task = client.CheckLocationSettings(settingsRequest);

            task.AddOnSuccessListener(new LocationSettingsSuccessListener(tcs));
            task.AddOnFailureListener(new LocationSettingsFailureListener(tcs, Platform.CurrentActivity));

            return await tcs.Task;
        }

        return false;
    }

    private async Task<Location> RequestSingleLocationUpdateAsync()
    {
        var locationRequest = new LocationRequest.Builder(Priority.PriorityHighAccuracy)
            .SetIntervalMillis(ONE_MINUTE)
            .SetWaitForAccurateLocation(false)
            .Build();

        var locationCallback = new SingleLocationCallback(_fusedLocationClient);
        await _fusedLocationClient.RequestLocationUpdatesAsync(locationRequest, locationCallback);

        var locationResult = await locationCallback.LocationTask;
        locationCallback.Dispose();
        return ToLocationModel(location: locationResult.LastLocation);
    }

    #region Helper Classes
    private class SingleLocationCallback : LocationCallback
    {
        private readonly IFusedLocationProviderClient _fusedLocationClient;
        private readonly TaskCompletionSource<LocationResult> _tcs;

        public SingleLocationCallback(IFusedLocationProviderClient fusedLocationClient)
        {
            _fusedLocationClient = fusedLocationClient;
            _tcs = new TaskCompletionSource<LocationResult>();
        }

        public override void OnLocationResult(LocationResult result)
        {
            base.OnLocationResult(result);
            _fusedLocationClient.RemoveLocationUpdates(this);
            _tcs.SetResult(result);
        }

        public Task<LocationResult> LocationTask => _tcs.Task;
    }

    private class LocationSettingsSuccessListener : Java.Lang.Object, IOnSuccessListener
    {
        private readonly TaskCompletionSource<bool> _tcs;

        public LocationSettingsSuccessListener(TaskCompletionSource<bool> tcs)
        {
            _tcs = tcs;
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            _tcs.TrySetResult(true);
        }
    }

    private class LocationSettingsFailureListener : Java.Lang.Object, IOnFailureListener
    {
        private readonly TaskCompletionSource<bool> _tcs;
        private readonly Activity _activity;

        public LocationSettingsFailureListener(TaskCompletionSource<bool> tcs, Activity activity)
        {
            _tcs = tcs;
            _activity = activity;
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            if (e is ResolvableApiException resolvable)
            {
                try
                {
                    if (MainActivity.LocationSettingsTcs == null || MainActivity.LocationSettingsTcs.Task.IsCompleted)
                    {
                        MainActivity.LocationSettingsTcs = new TaskCompletionSource<bool>();
                        resolvable.StartResolutionForResult(_activity, MainActivity.ACCURACY_LOCATION_REQUEST);
                    }

                    MainActivity.LocationSettingsTcs.Task.ContinueWith(task =>
                    {
                        _tcs.TrySetResult(task.Result);
                    });
                }
                catch
                {
                    MainActivity.LocationSettingsTcs?.TrySetResult(false);
                    _tcs.TrySetResult(false);
                }
            }
            else
            {
                _tcs.TrySetResult(false);
            }
        }
    }
    #endregion
}