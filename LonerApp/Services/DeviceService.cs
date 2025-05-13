using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.Views.InputMethods;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using AndroidX.Core.View;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Sharpnado.CollectionView.Droid.Helpers;

namespace LonerApp.Services
{
    public class DeviceService : IDeviceService
    {
        public static TaskCompletionSource<bool> RequestPermissionCompletionSource { get; set; }
        public System.IO.Stream GetRotatedImageStream(string path, bool isRotate = false, int maxDimension = 0)
        {
            if (File.Exists(path))
            {
                OrientationAndResize(path, maxDimension);
                return File.Open(path, FileMode.Open);
            }
            else
            {
                return null;
            }
        }
        public PermissionStatus GetNativePermissionStatus(Permission permission)
        {
            switch (permission)
            {
                case Permission.Photos:
                    return PermissionStatus.Granted;
                case Permission.Location:
                    return GetSelfPermission(Android.Manifest.Permission.AccessCoarseLocation) == Android.Content.PM.Permission.Granted ? PermissionStatus.Restricted : PermissionStatus.Denied;
                default:
                    return PermissionStatus.Denied;
            }
        }

                private Android.Content.PM.Permission GetSelfPermission(string permission)
        {
            var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity ?? throw new NullReferenceException("Current activity is null");
            return ContextCompat.CheckSelfPermission(activity, permission);
        }

        private void OrientationAndResize(string filePath, int maxDimension)
        {
            ExifInterface exif = new ExifInterface(filePath);
            try
            {
                var rotation = GetImageRotation(exif);
                if (rotation == 0)
                    return;

                var percent = 1.0f;

                var options = new BitmapFactory.Options
                {
                    InJustDecodeBounds = true
                };

                BitmapFactory.DecodeFile(filePath, options);

                var finalWidth = (int)(options.OutWidth * percent);
                var finalHeight = (int)(options.OutHeight * percent);

                options.InJustDecodeBounds = false;

                var originalImage = BitmapFactory.DecodeFile(filePath, options);

                if (originalImage == null)
                    return;

                if (finalWidth != originalImage.Width || finalHeight != originalImage.Height)
                {
                    originalImage = Bitmap.CreateScaledBitmap(originalImage, finalWidth, finalHeight, true);
                }

                if (rotation % 180 == 90)
                {
                    var a = finalWidth;
                    finalWidth = finalHeight;
                    finalHeight = a;
                }

                var photoType = System.IO.Path.GetExtension(filePath)?.ToLower();
                var compressFormat = photoType == ".png" ? Bitmap.CompressFormat.Png : Bitmap.CompressFormat.Jpeg;

                int newWidth = originalImage.Width;
                int newHeight = originalImage.Height;

                if (maxDimension > 0)
                {
                    if (originalImage.Height == originalImage.Width && originalImage.Width > maxDimension)
                    {
                        newWidth = newHeight = maxDimension;
                    }
                    else if (originalImage.Height > originalImage.Width && originalImage.Height > maxDimension)
                    {
                        newHeight = maxDimension;
                        newWidth = (int)(originalImage.Width * ((float)maxDimension / originalImage.Height));
                    }
                    else if (originalImage.Height < originalImage.Width && originalImage.Width > maxDimension)
                    {
                        newWidth = maxDimension;
                        newHeight = (int)(originalImage.Height * ((float)maxDimension / originalImage.Width));
                    }
                }

                Bitmap scaledImg = (newWidth != originalImage.Width || newHeight != originalImage.Height)
                    ? Bitmap.CreateScaledBitmap(originalImage, newWidth, newHeight, true)
                    : originalImage;

                if (rotation != 0)
                {
                    var matrix = new Matrix();
                    matrix.PostRotate(rotation);
                    using (var rotatedImage = Bitmap.CreateBitmap(scaledImg, 0, 0, newWidth, newHeight, matrix, true))
                    {
                        using (var stream = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite))
                        {
                            rotatedImage.Compress(compressFormat, 100, stream);
                            stream.Close();
                        }

                        rotatedImage.Recycle();
                    }

                    exif?.SetAttribute(ExifInterface.TagOrientation, Java.Lang.Integer.ToString((int)Android.Media.Orientation.Normal));
                }
                else
                {
                    using (var stream = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        scaledImg.Compress(compressFormat, 100, stream);
                        stream.Close();
                    }
                }

                originalImage.Recycle();
                originalImage.Dispose();
                scaledImg?.Recycle();
                scaledImg?.Dispose();
                GC.Collect();
                return;
            }
            catch (Exception)
            {
            }
        }

        static int GetImageRotation(ExifInterface exif)
        {
            if (exif == null)
                return 0;
            try
            {
                var orientation = (Android.Media.Orientation)exif.GetAttributeInt(ExifInterface.TagOrientation, (int)Android.Media.Orientation.Normal);

                switch (orientation)
                {
                    case Android.Media.Orientation.Rotate90:
                        return 90;
                    case Android.Media.Orientation.Rotate180:
                        return 180;
                    case Android.Media.Orientation.Rotate270:
                        return 270;
                    default:
                        return 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public double GetStatusBarHeight()
         => GetHeightByResourceId("status_bar_height");

        public double GetNavigationBarHeight()
            => GetHeightByResourceId("navigation_bar_height");

        private double GetHeightByResourceId(string resourceId)
        {
            int resourceIdInt = Android.App.Application.Context.Resources.GetIdentifier(resourceId, "dimen", "android");
            if (resourceIdInt > 0)
            {
                var result = Android.App.Application.Context.Resources.GetDimensionPixelSize(resourceIdInt);
                // Convert to dp
                float scale = Android.App.Application.Context.Resources.DisplayMetrics.Density;
                result = (int)((result / scale) + 0.5f);
                return result;
            }

            return 0;
        }

        public bool IsSoftKeyboardVisible(Android.Views.View view)
        {
            if (view != null)
            {
                var insets = ViewCompat.GetRootWindowInsets(view);
                if (insets == null)
                    return false;
                var result = insets.IsVisible(WindowInsetsCompat.Type.Ime());
                return result;
            }

            return false;
        }

        public void SetResizeKeyboardInput()
        {
            App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        }

        public void HideKeyboard()
        {
            var context = Android.App.Application.Context;
            var inputMethodManager = context.GetSystemService(Context.InputMethodService) as InputMethodManager;
            if (inputMethodManager != null)
            {
                var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
                var token = activity.CurrentFocus?.WindowToken;
                inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);
                if (inputMethodManager.IsNullOrDisposed())
                    return;
                activity.Window.DecorView.ClearFocus();
                var isKeyboardShow = IsSoftKeyboardVisible(activity.Window.DecorView.RootView);
                if (isKeyboardShow)
                    inputMethodManager.HideSoftInputFromWindow(activity.Window.DecorView.RootView.WindowToken, HideSoftInputFlags.None);
            }
        }

        public async Task<bool> RegisterForPushNotificationsAsync()
        {
            RequestPermissionCompletionSource = new TaskCompletionSource<bool>();
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                var requiredPermissions = new string[] { Android.Manifest.Permission.PostNotifications };
                var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
                try
                {
                    if (!ActivityCompat.ShouldShowRequestPermissionRationale(activity, Android.Manifest.Permission.PostNotifications))
                    {
                        ActivityCompat.RequestPermissions(activity, requiredPermissions, 1);
                    }
                    else
                    {
                        RequestPermissionCompletionSource.TrySetResult(false);
                    }
                }
                catch (Exception)
                {
                    RequestPermissionCompletionSource.TrySetResult(false);
                }
            }

            // if (RequestPermissionCompletionSource != null && !RequestPermissionCompletionSource.Task.IsCompleted)
            // {
            //     await RequestPermissionCompletionSource.Task;
            // }

            var notificationManager = (NotificationManager)Android.App.Application.Context.GetSystemService(Context.NotificationService);
            return notificationManager.AreNotificationsEnabled();
        }
    }
}