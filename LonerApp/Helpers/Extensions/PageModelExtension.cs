namespace LonerApp.Helpers.Extensions
{
    public static class PageModelExtension
    {
        private static TaskCompletionSource<bool>? _permissionTaskSource;

        public static async Task<bool> ShowAlertRequestPermissionAsync(this BasePageModel pageModel, string permissionName)
        {
            if (_permissionTaskSource != null && !_permissionTaskSource.Task.IsCompleted)
                return await _permissionTaskSource.Task;

            _permissionTaskSource = new TaskCompletionSource<bool>();

            string contentAlert;

            switch (permissionName)
            {
                case "Camera":
                    contentAlert = I18nHelper.Get("Alert_Camera_GotoSeting");
                    break;
                case "Location":
                    contentAlert = I18nHelper.Get("Alert_Location_GotoSeting");
                    break;
                case "Photos":
                case "Storage":
                    contentAlert = I18nHelper.Get("Alert_Photo_GotoSeting");
                    break;
                default:
                    contentAlert = string.Format(I18nHelper.Get("Common_Alert_AllowPermission"), permissionName);
                    break;
            }

            if(await AlertHelper.ShowConfirmationAlertAsync(contentAlert, I18nHelper.Get("Common_Alert_GotoSetting")))
            {
                ServiceHelper.GetService<IOpenSetting>().OpenSettingScreen();
                _permissionTaskSource.TrySetResult(true);
            }
            else
            {
                _permissionTaskSource.TrySetResult(false);
            }

            return await _permissionTaskSource.Task;
        }

        public static async Task<bool> AllowTakePhotoAsync(this BasePageModel pageModel)
        {
            List<Permission> permissions = new List<Permission>
                {
                    Permission.Camera
                };

            permissions.Add(Permission.StorageRead);
            permissions.Add(Permission.StorageWrite);

            return await AllowPermissionsAsync(pageModel, permissions);
        }

        private static async Task<bool> AllowPermissionsAsync(BasePageModel pageModel, List<Permission> permissions)
        {
            var allPermissionsAllowed = true;
            foreach (Permission p in permissions)
            {
                if (await CheckPermission.CheckPermissionAsync(p) == PermisionResultKey.GoToSetting)
                {
                    allPermissionsAllowed = false;
                    await pageModel.ShowAlertRequestPermissionAsync(p.ToString());
                    break;
                }

                var statusPermission = await CheckPermission.CheckPermissionStatusAsync(p);
                if(statusPermission != PermissionStatus.Granted)
                {
                    allPermissionsAllowed = false;
                    break;
                }
            }

            return allPermissionsAllowed;
        }
    }
}