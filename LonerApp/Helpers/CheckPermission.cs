
namespace LonerApp.Helpers
{
    public enum PermisionResultKey
    {
        None,
        GoToSetting
    }

    public enum Permission
    {
        Camera,
        Location,
        StorageRead,
        StorageWrite,
        Photos
    }

    public static class CheckPermission
    {
        private static List<Permission> _goToSettingPermissions;
        private static TaskCompletionSource<PermisionResultKey> _permissionTaskSource;
        static List<Permission> GoToSettingPermissions
        {
            get
            {
                if (_goToSettingPermissions == null)
                {
                    _goToSettingPermissions = UserSetting.GetObject<List<Permission>>(StorageKey.PermissionRationaleStatus) ?? new List<Permission>();
                }

                return _goToSettingPermissions;
            }
        }

        public static async Task<PermisionResultKey> CheckPermissionAsync(Permission permissionName)
        {
            if(_permissionTaskSource != null && !_permissionTaskSource.Task.IsCompleted)
            {
                return await _permissionTaskSource.Task;
            }

            _permissionTaskSource = new TaskCompletionSource<PermisionResultKey>();
            var statusPermission = await CheckPermissionStatusAsync(permissionName);
            if(statusPermission != PermissionStatus.Granted || statusPermission != PermissionStatus.Restricted)
            {
                if(NeverAskAgainSelected(permissionName))
                {
                    try
                    {
                        var statusPermissionCheckAgain = await RequestPermissionAsync(permissionName);
                        if(statusPermissionCheckAgain == PermissionStatus.Granted || statusPermissionCheckAgain == PermissionStatus.Restricted)
                        {
                            RemoveGoToSettingPermission(permissionName);
                            _permissionTaskSource.TrySetResult(PermisionResultKey.None);
                            return await _permissionTaskSource.Task;
                        }
                        else if(statusPermissionCheckAgain == PermissionStatus.Denied)
                        {
                            var currentShouldShowStatus = CheckShouldShowRationale(permissionName);
                            if (currentShouldShowStatus)
                            {
                                _permissionTaskSource.TrySetResult(PermisionResultKey.None);
                                return await _permissionTaskSource.Task;
                            }
                            else
                            {
                                _permissionTaskSource.TrySetResult(PermisionResultKey.GoToSetting);
                                return await _permissionTaskSource.Task;
                            }
                        }
                        else
                        {
                            _permissionTaskSource.TrySetResult(PermisionResultKey.GoToSetting);
                            return await _permissionTaskSource.Task;
                        }
                    }
                    catch (TaskCanceledException)
                    {
                    }
                }
                else
                {
                    try
                    {
                        var statusPermissionCheckAgain = await RequestPermissionAsync(permissionName);
                        if (statusPermissionCheckAgain == PermissionStatus.Denied)
                            AddGoToSettingPermission(permissionName);
                    }
                    catch (TaskCanceledException)
                    {
                    }
                }
            }
            else
            {
                RemoveGoToSettingPermission(permissionName);
            }

            _permissionTaskSource.TrySetResult(PermisionResultKey.None);
            return await _permissionTaskSource.Task;
        }

        private static async Task<PermissionStatus> RequestPermissionAsync(Permission permissionName)
        {
            switch (permissionName)
            {
                case Permission.StorageRead:
                    return await Permissions.RequestAsync<Permissions.StorageRead>();
                case Permission.StorageWrite:
                    return await Permissions.RequestAsync<Permissions.StorageWrite>();
                case Permission.Camera:
                    return await Permissions.RequestAsync<Permissions.Camera>();
                case Permission.Location:
                    return await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                case Permission.Photos:
                    return PermissionStatus.Granted;
                default:
                    return PermissionStatus.Unknown;
            }
        }

        public static async Task<PermissionStatus> CheckPermissionStatusAsync(Permission permissionName)
        {
            switch(permissionName)
            {
                case Permission.StorageRead:
                    return await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                case Permission.StorageWrite:
                    return await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                case Permission.Camera:
                    return await Permissions.CheckStatusAsync<Permissions.Camera>();
                case Permission.Location:
                    return await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                case Permission.Photos:
                    return PermissionStatus.Granted;
                default:
                    return PermissionStatus.Unknown;
            }
        }

        private static bool CheckShouldShowRationale(Permission permission)
        {
            switch(permission)
            {
                case Permission.StorageRead:
                    return Permissions.ShouldShowRationale<Permissions.StorageRead>();
                case Permission.StorageWrite:
                    return Permissions.ShouldShowRationale<Permissions.StorageWrite>();
                case Permission.Camera:
                    return Permissions.ShouldShowRationale<Permissions.Camera>();
                case Permission.Location:
                    return Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>();
                case Permission.Photos:
                    return true;
                default:
                    return false;
            }
        }

        private static bool NeverAskAgainSelected(Permission permission)
        {
            var prevShouldShowStatus = GoToSettingPermissions.Contains(permission);
            var currentShouldShowStatus = CheckShouldShowRationale(permission);
            return prevShouldShowStatus != currentShouldShowStatus;
        }

        private static void RemoveGoToSettingPermission(Permission permissionName)
        {
            if(GoToSettingPermissions.Contains(permissionName))
            {
                GoToSettingPermissions.Remove(permissionName);
                UserSetting.SetObject(StorageKey.PermissionRationaleStatus, GoToSettingPermissions);
            }
        }

        private static void AddGoToSettingPermission(Permission permissionName)
        {
            if(!GoToSettingPermissions.Contains(permissionName))
            {
                GoToSettingPermissions.Add(permissionName);
                UserSetting.SetObject(StorageKey.PermissionRationaleStatus, GoToSettingPermissions);
            }
        }
    }
}