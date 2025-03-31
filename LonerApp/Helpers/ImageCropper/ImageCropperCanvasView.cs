using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace LonerApp.Helpers.ImageCropper
{
    public class ImageCropperCanvasView : SKCanvasView
    {
        const int CORNER = 50;
        const float DEFAULT_ASPACT_RATIO = 4f / 3f;

        SKBitmap _bitmap;
        CroppingRectangle _croppingRect;
        double _scaleFactor = 1;
        SKPoint _translation = new SKPoint(0, 0);

        SKPaint _cornerStroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.LightGray,
            StrokeWidth = 10
        };

        SKPaint _edgeStroke = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.LightGray,
            StrokeWidth = 2
        };

        public ImageCropperCanvasView(SKBitmap bitmap, float? aspectRatio = DEFAULT_ASPACT_RATIO)
        {
            this._bitmap = bitmap;
            this.BackgroundColor = Colors.Gray;
            SKRect bitmapRect = new SKRect(0, 0, bitmap.Width, bitmap.Height);
            _croppingRect = new CroppingRectangle(bitmapRect, aspectRatio);

            var pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += (s, e) =>
            {
                if (e.Status == GestureStatus.Running)
                {
                    _translation.X = _translation.X - (float)(e.ScaleOrigin.X * Width * _scaleFactor * (e.Scale - 1));
                    _translation.Y = _translation.Y - (float)(e.ScaleOrigin.Y * Height * _scaleFactor * (e.Scale - 1));
                    _scaleFactor = _scaleFactor * e.Scale;
                    InvalidateSurface();
                }
            };

            var panGesture = new PanGestureRecognizer();
            float lastX = 0;
            float lastY = 0;
            bool shouldTranslate = false;

            panGesture.PanUpdated += (s, e) =>
            {
                if (e.StatusType == GestureStatus.Started)
                {
                    shouldTranslate = true;
                }
                else if (e.StatusType == GestureStatus.Running)
                {
                    if (shouldTranslate)
                    {
                        _translation.X += (float)e.TotalX - lastX;
                        _translation.Y += (float)e.TotalY - lastY;
                        lastX = (float)e.TotalX;
                        lastY = (float)e.TotalY;

                        InvalidateSurface();
                    }
                }
                else
                {
                    shouldTranslate = false;
                    lastX = 0;
                    lastY = 0;
                }
            };

            GestureRecognizers.Add(pinchGesture);
            GestureRecognizers.Add(panGesture);
        }

        public SKBitmap CroppedBitmap
        {
            get
            {
                SKRect cropRect = _croppingRect.Rect;
                SKBitmap croppedBitmap = new SKBitmap((int)cropRect.Width, (int)cropRect.Height);
                float rate = Math.Max(
                    (float)(_bitmap.Height / (Height * _scaleFactor)),
                    (float)(_bitmap.Width / (Width * _scaleFactor)));

                SKRect dest = new SKRect(0, 0, cropRect.Width, cropRect.Height);
                SKRect source = SKRect.Create(
                    (float)(cropRect.Left / _scaleFactor) - (_translation.X * rate),
                    (float)(cropRect.Top / _scaleFactor) - (_translation.Y * rate),
                    (float)(cropRect.Width / _scaleFactor),
                    (float)(cropRect.Height / _scaleFactor));

                using (SKCanvas canvas = new SKCanvas(croppedBitmap))
                {
                    canvas.Clear(SKColor.Empty);
                    canvas.DrawBitmap(_bitmap, source, dest);
                }

                return croppedBitmap;
            }
        }

        public bool IsValidCropArea
        {
            get
            {
                SKRect cropRect = _croppingRect.Rect;
                float rate = Math.Max(
                    (float)(_bitmap.Height / (Height * _scaleFactor)),
                    (float)(_bitmap.Width / (Width * _scaleFactor)));

                SKRect source = SKRect.Create(
                    (float)(cropRect.Left / _scaleFactor) - (_translation.X * rate),
                    (float)(cropRect.Top / _scaleFactor) - (_translation.Y * rate),
                    (float)(cropRect.Width / _scaleFactor),
                    (float)(cropRect.Height / _scaleFactor));

                return source.Right > 0 && source.Left < _bitmap.Width && source.Bottom > 0 && source.Top < _bitmap.Height;
            }
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);
            using (var canvas = e.Surface.Canvas)
            {
                SKImageInfo info = e.Info;
                float rate = (float)(canvas.LocalClipBounds.Width / Width);

                canvas.Clear(SKColors.Transparent);

                float scale = Math.Min((float)info.Width / _bitmap.Width, (float)info.Height / _bitmap.Height);
                float x = (info.Width - (scale * _bitmap.Width)) / 2;
                float y = (info.Height - (scale * _bitmap.Height)) / 2;

                SKRect bitmapRect = new SKRect(
                    x + (_translation.X * rate),
                    y + (_translation.Y * rate),
                    x + (scale * _bitmap.Width * (float)_scaleFactor) + (_translation.X * rate),
                    y + (scale * _bitmap.Height * (float)_scaleFactor) + (_translation.Y * rate));

                canvas.DrawBitmap(_bitmap, bitmapRect);

                SKMatrix bitmapScaleMatrix = SKMatrix.CreateScaleTranslation(scale, scale, x, y);
                SKRect scaledCropRect = bitmapScaleMatrix.MapRect(_croppingRect.Rect);
                canvas.DrawRect(scaledCropRect, _edgeStroke);

                using (SKPath path = new SKPath())
                {
                    path.MoveTo(scaledCropRect.Left, scaledCropRect.Top + CORNER);
                    path.LineTo(scaledCropRect.Left, scaledCropRect.Top);
                    path.LineTo(scaledCropRect.Left + CORNER, scaledCropRect.Top);

                    path.MoveTo(scaledCropRect.Right - CORNER, scaledCropRect.Top);
                    path.LineTo(scaledCropRect.Right, scaledCropRect.Top);
                    path.LineTo(scaledCropRect.Right, scaledCropRect.Top + CORNER);

                    path.MoveTo(scaledCropRect.Right, scaledCropRect.Bottom - CORNER);
                    path.LineTo(scaledCropRect.Right, scaledCropRect.Bottom);
                    path.LineTo(scaledCropRect.Right - CORNER, scaledCropRect.Bottom);

                    path.MoveTo(scaledCropRect.Left + CORNER, scaledCropRect.Bottom);
                    path.LineTo(scaledCropRect.Left, scaledCropRect.Bottom);
                    path.LineTo(scaledCropRect.Left, scaledCropRect.Bottom - CORNER);

                    canvas.DrawPath(path, _cornerStroke);
                }
            }
        }
    }
}