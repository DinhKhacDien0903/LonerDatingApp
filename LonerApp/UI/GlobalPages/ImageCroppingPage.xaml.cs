using LonerApp.Helpers.ImageCropper;
using Mopups.Pages;
using SkiaSharp;
using SkiaSharp.Views.Maui;
namespace LonerApp.UI.GlobalPages;

public partial class ImageCroppingPage : PopupPage
{
    private CancellationTokenSource _cancellationTokenSource;
    private const int _pressDelayMillis = 100;
    bool _isProcess;
    SKBitmap _croppedBitmap;
    ImageCropperCanvasView _imageCropper;
    Action<byte[]> _finishCroppingCallback;
    public TaskCompletionSource<byte[]> DoneTask { get; }

    public ImageCroppingPage(SKBitmap bitmap, Action<byte[]> finishCallback = null)
    {
        InitializeComponent();
        DoneTask = new TaskCompletionSource<byte[]>();
        _finishCroppingCallback = finishCallback;
        _croppedBitmap = bitmap;
        _imageCropper = new ImageCropperCanvasView(bitmap);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        canvasViewHost.Children.Add(_imageCropper);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (!DoneTask.Task.IsCompleted)
            DoneTask.TrySetResult(null);
    }

    private void OnRotateButtonClicked(object sender, EventArgs e)
    {
        if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
        {
            _cancellationTokenSource.Cancel();
        }

        _cancellationTokenSource = new CancellationTokenSource();
        Task.Delay(_pressDelayMillis, _cancellationTokenSource.Token)
            .ContinueWith(t =>
            {
                if (!t.IsCanceled)
                {
                    SKBitmap rotatedBitmap = new SKBitmap(_croppedBitmap.Height, _croppedBitmap.Width);

                    try
                    {
                        using (SKCanvas canvas = new SKCanvas(rotatedBitmap))
                        {
                            canvas.Clear();
                            canvas.Translate(_croppedBitmap.Height, 0);
                            canvas.RotateDegrees(90);
                            canvas.DrawBitmap(_croppedBitmap, new SKPoint());
                        }

                        _croppedBitmap = rotatedBitmap;
                        _imageCropper = new ImageCropperCanvasView(rotatedBitmap);
                        canvasViewHost.Clear();
                        canvasViewHost.Children.Add(_imageCropper);
                    }
                    finally
                    {
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                    }
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    async void OnDoneButtonClickedAsync(object sender, EventArgs args)
    {
        if (_isProcess)
        {
            return;
        }

        _isProcess = true;
        if (!_imageCropper.IsValidCropArea)
        {
            await AlertHelper.ShowErrorAlertAsync(
                I18nHelper.Get("ImageCroppingPage_ErrorMessage_CropPhoto"),
                I18nHelper.Get("ImageCroppingPage_ErrorTitle_CropPhoto"));
            _isProcess = false;
            return;
        }

        indicator.IsRunning = true;
        indicator.IsVisible = true;

        var bytes = await Task.Run(() =>
        {
            _croppedBitmap = _imageCropper.CroppedBitmap;
            SKImage image = SKImage.FromBitmap(_croppedBitmap);
            return image.Encode().ToArray();
        });

        _finishCroppingCallback?.Invoke(bytes);
        DoneTask.TrySetResult(bytes);

        indicator.IsRunning = false;
        indicator.IsVisible = false;
        root.BackgroundColor = Colors.Transparent;
        _isProcess = false;
    }

    private void OnCancelButtonClicked(object sender, EventArgs e)
    {
        canvasViewHost.Clear();
        DoneTask.TrySetResult(null);
        root.BackgroundColor = Colors.Transparent;
    }

    protected override bool OnBackButtonPressed()
    {
        DoneTask.TrySetResult(null);
        return true;
    }

    public void SetFileResult(Stream file)
    {
        SKBitmap bitmap = LoadBitmap(file);
        canvasViewHost.Children.Clear();
        _imageCropper = new ImageCropperCanvasView(bitmap);
        canvasViewHost.Children.Add(_imageCropper);
    }

    void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
    {
        using (var canvas = args.Surface.Canvas)
        {
            SKImageInfo info = args.Info;
            canvas.Clear();
            DrawBitmap(canvas, _croppedBitmap, info.Rect);
        }
    }

    static SKBitmap LoadBitmap(Stream file)
    {
        using (file)
        {
            return SKBitmap.Decode(file);
        }
    }

    static void DrawBitmap(SKCanvas canvas, SKBitmap bitmap, SKRect dest, SKPaint paint = null)
    {
        float scale = 1;

        scale = Math.Min(dest.Width / bitmap.Width, dest.Height / bitmap.Height);

        SKRect display = CalculateDisplayRect(dest, scale * bitmap.Width, scale * bitmap.Height);

        canvas.DrawBitmap(bitmap, display, paint);
    }

    static SKRect CalculateDisplayRect(SKRect dest, float bmpWidth, float bmpHeight)
    {
        float x = 0;
        float y = 0;

        x = (dest.Width - bmpWidth) / 2;
        y = (dest.Height - bmpHeight) / 2;

        x += dest.Left;
        y += dest.Top;

        return new SKRect(x, y, x + bmpWidth, y + bmpHeight);
    }
}