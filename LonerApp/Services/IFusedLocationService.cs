namespace LonerApp.Services;
public interface IFusedLocationService
{
    Task<Location> RequestLocationUpdatesAsync();
}