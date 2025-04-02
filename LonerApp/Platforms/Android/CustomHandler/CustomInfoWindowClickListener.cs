using Android.Gms.Maps;
using Android.Gms.Maps.Model;
namespace LonerApp.Platforms.Android.CustomHandler;

internal class CustomInfoWindowClickListener(CustomMapHandler mapHandler)
	: Java.Lang.Object, GoogleMap.IOnInfoWindowClickListener
{
	public void OnInfoWindowClick(Marker marker)
	{
		var pin = mapHandler.Markers.FirstOrDefault(x => x.marker.Id == marker.Id);
		pin.pin?.SendInfoWindowClick();
	}
}
