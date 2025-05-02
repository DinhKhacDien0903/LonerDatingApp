namespace LonerApp.Models
{
    public class UpdateLocationRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
        public string Latitude { get; set; } = string.Empty;
    }
}