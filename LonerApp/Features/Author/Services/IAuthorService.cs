using LonerApp.Features.Author.Login.Models;

namespace LonerApp.Services;

public interface IAuthorService
{
    public Task<SendOtpResponse?> SendMailOtpAsync(LoginRequest request);
    public Task<LoginResponse?> VerifyEmailAsync(VerifyEmailRequest request);
}