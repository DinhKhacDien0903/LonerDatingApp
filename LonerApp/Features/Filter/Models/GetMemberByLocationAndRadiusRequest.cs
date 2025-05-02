namespace LonerApp.Models
{
    public class GetMemberByLocationAndRadiusRequest
    {
        public string UserId { get; init; } = string.Empty;
        public string Longitude { get; init; } = string.Empty;
        public string Latitude { get; init; } = string.Empty;
        public double Radius { get; init; }
    }
}