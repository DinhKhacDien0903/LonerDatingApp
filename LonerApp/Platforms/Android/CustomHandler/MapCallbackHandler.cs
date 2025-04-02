using Android.Gms.Maps;
using Microsoft.Maui.Maps;
namespace LonerApp.Platforms.Android.CustomHandler;

class MapCallbackHandler(CustomMapHandler mapHandler) : Java.Lang.Object, IOnMapReadyCallback
{
	public void OnMapReady(GoogleMap googleMap)
	{
		mapHandler.UpdateValue(nameof(Microsoft.Maui.Maps.IMap.Pins));
		mapHandler.Map?.SetOnMarkerClickListener(new CustomMarkerClickListener(mapHandler));
		mapHandler.Map?.SetOnInfoWindowClickListener(new CustomInfoWindowClickListener(mapHandler));
	}
}
