using Android.Graphics;
using Android.Media;

namespace LonerApp.Services
{
    public class DeviceService : IDeviceService
    {
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
    }
}