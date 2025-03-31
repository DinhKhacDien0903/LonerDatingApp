using SkiaSharp;
namespace LonerApp.Helpers.ImageCropper
{
    public class CroppingRectangle
    {
        const float MINIMUM = 10;

        SKRect _maxRect;
        float? _aspectRatio;

        public CroppingRectangle(SKRect maxRect, float? aspectRatio = null)
        {
            this._maxRect = maxRect;
            this._aspectRatio = aspectRatio;

            Rect = new SKRect(
                _maxRect.Left,
                _maxRect.Top,
                _maxRect.Right,
                _maxRect.Bottom);

            if (aspectRatio.HasValue)
            {
                SKRect rect = Rect;
                float aspect = aspectRatio.Value;

                if (rect.Width > aspect * rect.Height)
                {
                    float width = aspect * rect.Height;
                    rect.Left = (_maxRect.Width - width) / 2;
                    rect.Right = rect.Left + width;
                }
                else
                {
                    float height = rect.Width / aspect;
                    rect.Top = (_maxRect.Height - height) / 2;
                    rect.Bottom = rect.Top + height;
                }

                Rect = rect;
            }
        }

        public SKRect Rect { get; set; }

        public SKPoint[] Corners
        {
            get
            {
                return new SKPoint[]
                {
                    new SKPoint(Rect.Left, Rect.Top),
                    new SKPoint(Rect.Right, Rect.Top),
                    new SKPoint(Rect.Right, Rect.Bottom),
                    new SKPoint(Rect.Left, Rect.Bottom)
                };
            }
        }
    }
}