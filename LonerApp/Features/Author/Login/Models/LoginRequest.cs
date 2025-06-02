namespace LonerApp.Models;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public bool IsLoggingIn { get; set; }
}

public class VerifyEmailRequest
{
    public string Email { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
    public bool IsLoggingIn { get; set; } = true;
}