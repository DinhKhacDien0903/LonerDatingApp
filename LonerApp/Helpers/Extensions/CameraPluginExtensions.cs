
using LonerApp.UI.GlobalPages;
using Mopups.Interfaces;
using SkiaSharp;

namespace LonerApp.Helpers.Extensions
{
    public static class CameraPluginExtensions
    {
        public static string ImagePath = string.Empty;

        public static async Task<string> CancelableTakePhotoAsync(string filename = null)
        {
            try
            {
                var fileResult = await MediaPicker.CapturePhotoAsync();
                await LoadImageAsync(fileResult, filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return ImagePath;
        }

        public static async Task<string> CancelableChoosePhotoAsync(string filename = null)
        {
            try
            {
                var fileResult = await MediaPicker.PickPhotoAsync();
                await LoadImageAsync(fileResult, filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ImagePath;
        }

        /// <summary>
        /// Copies the selected image file to the device's cache directory.
        /// </summary>
        /// <param name="fileResult">The selected file result, can be null.</param>
        /// <param name="filename">The filename to be used when saving to the cache.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method will:
        /// - Check if the input file is not null
        /// - Create a full path in the cache directory
        /// - Copy data from the source file to the cache
        /// - Store the cache path in the PhotoPath property for later use
        /// </remarks>
        private static async Task LoadImageAsync(FileResult? fileResult, string filename)
        {
            if (fileResult == null)
            {
                ImagePath = string.Empty;
                return;
            }

            //create path to save filename in cache device
            var newFile = Path.Combine(FileSystem.CacheDirectory, filename);
            //open file to read
            using (var stream = await fileResult.OpenReadAsync())
            // create new file in cache directory to write data
            using (var newStream = File.OpenWrite(newFile))
                // copy data from source file to new file
                await stream.CopyToAsync(newStream);

            ImagePath = newFile;
        }

        public static async Task<string> TrimImageAsync(Stream file, string filename)
        {
            await Task.Delay(100);
            var bitmap = SKBitmap.Decode(file);
            if (bitmap == null)
            {
                await AlertHelper.ShowErrorAlertAsync(
                    I18nHelper.Get("PhotoCroppingPage_ErrorMessage_DecodeBitmap"));

                return string.Empty;
            }

            var crop = new ImageCroppingPage(bitmap);
            await ServiceHelper.GetService<IPopupNavigation>().PushAsync(crop);
            var dataBytes = await crop.DoneTask.Task;
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await ServiceHelper.GetService<IPopupNavigationService>().PopAsync();
            });

            if (dataBytes != null)
            {
                using (var outFileStream = new FileStream(filename, FileMode.Create, FileAccess.Write))
                using (var sourceStream = new MemoryStream(dataBytes))
                {
                    await sourceStream.CopyToAsync(outFileStream);
                }

                return filename;
            }

            return string.Empty;
        }
    }
}