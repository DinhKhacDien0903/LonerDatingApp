namespace LonerApp.Models
{
    public class GetMemberByLocationAndRadiusResponse
    {
        public List<UserLocationDto> Users { get; set; } = [];
    }

    public class UserLocationDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; //Format: Address or University - Distance
        public string? AvatarUrl { get; set; }
        public string Longitude { get; set; } = string.Empty;
        public string Latitude { get; set; } = string.Empty;
    }
}