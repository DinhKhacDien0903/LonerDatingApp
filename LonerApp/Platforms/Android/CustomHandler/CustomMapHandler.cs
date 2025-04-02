using Microsoft.Maui.Maps.Handlers;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Graphics.Drawables;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Platform;
using Paint = Android.Graphics.Paint;
using Rect = Android.Graphics.Rect;
using RectF = Android.Graphics.RectF;
using IMap = Microsoft.Maui.Maps.IMap;

namespace LonerApp.Platforms.Android.CustomHandler;

public class CustomMapHandler : MapHandler
{
	public static readonly IPropertyMapper<IMap, IMapHandler> CustomMapper =
		new PropertyMapper<IMap, IMapHandler>(Mapper)
		{
			[nameof(IMap.Pins)] = MapPins
		};

	public CustomMapHandler() : base(CustomMapper, CommandMapper)
	{
	}

    public CustomMapHandler(IPropertyMapper? mapper = null, CommandMapper? commandMapper = null) : base(
    mapper ?? CustomMapper, commandMapper ?? CommandMapper)
	{
	}

	protected override void ConnectHandler(MapView platformView)
	{
		base.ConnectHandler(platformView);
		var mapReady = new MapCallbackHandler(this);
		PlatformView.GetMapAsync(mapReady);
	}

    public List<(IMapPin pin, Marker marker)> Markers { get; } = new();

    private static new void MapPins(IMapHandler handler, IMap map)
	{
		if (handler is CustomMapHandler mapHandler)
		{
			var pinsToAdd = map.Pins.Where(x => x.MarkerId == null).ToList();
			var pinsToRemove = mapHandler.Markers.Where(x => !map.Pins.Contains(x.pin)).ToList();
			foreach (var marker in pinsToRemove)
			{
				marker.marker.Remove();
				mapHandler.Markers.Remove(marker);
			}

			mapHandler.AddPins(pinsToAdd);
		}
	}

    private void AddPins(IEnumerable<IMapPin> mapPins)
	{
		if (Map is null || MauiContext is null)
		{
			return;
		}

		foreach (var pin in mapPins)
		{
			var pinHandler = pin.ToHandler(MauiContext);
			if (pinHandler is IMapPinHandler mapPinHandler)
			{
				var markerOption = mapPinHandler.PlatformView;
				if (pin is CustomPin cp)
				{
					cp.ImageSource.LoadImage(MauiContext, result =>
					{
						if (result?.Value is BitmapDrawable { Bitmap: not null } bitmapDrawable)
						{
							markerOption.SetIcon(BitmapDescriptorFactory.FromBitmap(GetMaximumBitmap(bitmapDrawable.Bitmap, 100, 100)));
						}

						AddMarker(Map, pin, markerOption);
					});
				}
				else
				{
					AddMarker(Map, pin, markerOption);
				}
			}
		}
	}

    private void AddMarker(GoogleMap map, IMapPin pin, MarkerOptions markerOption)
	{
		var marker = map.AddMarker(markerOption);
		pin.MarkerId = marker.Id;
		Markers.Add((pin, marker));
	}

    private static Bitmap GetMaximumBitmap(in Bitmap sourceImage, in float maxWidth, in float maxHeight)
	{
		var sourceSize = new Size(sourceImage.Width, sourceImage.Height);
		var maxResizeFactor = Math.Min(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);

		float width =(float) Math.Max(maxResizeFactor * sourceSize.Width, 1);
		float height =(float)  Math.Max(maxResizeFactor * sourceSize.Height, 1);
		// return Bitmap.CreateScaledBitmap(sourceImage, (int)width, (int)height, false)
		// 		?? throw new InvalidOperationException("Failed to create Bitmap");
		// 		Bitmap scaledBitmap = Bitmap.CreateScaledBitmap(sourceImage, (int)width, (int)height, false)
        //     ?? throw new InvalidOperationException("Failed to create scaled Bitmap");

        Bitmap scaledBitmap = Bitmap.CreateScaledBitmap(sourceImage, (int)width, (int)height, false)
                ?? throw new InvalidOperationException("Failed to create scaled Bitmap");

        Bitmap output = Bitmap.CreateBitmap((int)width, (int)height, Bitmap.Config.Argb8888)
                ?? throw new InvalidOperationException("Failed to create output Bitmap");

        Canvas canvas = new Canvas(output);

        Paint paint = new Paint();
        Rect rect = new Rect(0, 0, (int)width, (int)height);
        RectF rectF = new RectF(rect);

        paint.AntiAlias = true;
        canvas.DrawARGB(0, 0, 0, 0);
        paint.Color = global::Android.Graphics.Color.ParseColor("#BAB399");

        canvas.DrawCircle(width / 2f, height / 2f, Math.Min(width, height) / 2f, paint);

        paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
        canvas.DrawBitmap(scaledBitmap, rect, rectF, paint);

        scaledBitmap.Recycle();
        return output;
	}
}
