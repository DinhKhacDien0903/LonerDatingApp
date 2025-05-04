namespace LonerApp.Services;

public interface IAuthorService
{
    public Task<SendOtpResponse?> SendMailOtpAsync(LoginRequest request);
    public Task<LoginResponse?> VerifyEmailAsync(VerifyEmailRequest request);
    public Task<LogoutResponse?> LogoutAsync(LogoutRequest request);
}