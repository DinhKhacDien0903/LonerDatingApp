namespace LonerApp.Models;

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public bool IsSetupedAccount { get; set; }
    public bool IsAccountExisted { get; set; }
}