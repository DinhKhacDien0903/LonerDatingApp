using Android.Gms.Maps;
using Android.Gms.Maps.Model;

namespace LonerApp.Platforms.Android.CustomHandler;

internal class CustomMarkerClickListener(CustomMapHandler mapHandler)
	: Java.Lang.Object, GoogleMap.IOnMarkerClickListener
{
	public bool OnMarkerClick(Marker marker)
	{
		var pin = mapHandler.Markers.FirstOrDefault(x => x.marker.Id == marker.Id);
		pin.pin?.SendMarkerClick();
		marker.ShowInfoWindow();
		return true;
	}
}
