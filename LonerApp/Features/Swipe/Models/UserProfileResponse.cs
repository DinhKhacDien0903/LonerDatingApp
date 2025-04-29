namespace LonerApp.Models;

public class GetProfilesResponse
{
    public PaginatedResponse<UserProfileResponse>? User { get; set; }
}

public class UserProfileResponse
{
    public string Id { get; set; } = string.Empty;
    public string MatchId { get; set; } = string.Empty;
    public string? Username { get; set; }
    public int? Age { get; set; }
    public string AvatarUrl { get; set; } = string.Empty;
    public IEnumerable<string>? Interests { get; set; }
}