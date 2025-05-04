namespace LonerApp.Services;

public class AuthorService : IAuthorService
{
    private readonly IApiService _apiService;

    public AuthorService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<LogoutResponse?> LogoutAsync(LogoutRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.UserId.Trim()))
                return new LogoutResponse
                {
                    Message = "UserId cannot be empty",
                    IsSuccess = false
                };
            if (string.IsNullOrEmpty(request.RefreshToken.Trim()))
                return new LogoutResponse
                {
                    Message = "RefreshToken cannot be empty",
                    IsSuccess = false
                };

            return await _apiService.PostAsync<LogoutResponse>(EnvironmentsExtensions.ENDPOINT_LOGOUT, request);
        }
        catch (Exception ex)
        {
            return new LogoutResponse
            {
                Message = ex.Message,
                IsSuccess = false
            };
        }
    }

    public async Task<SendOtpResponse?> SendMailOtpAsync(LoginRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Email.Trim()))
                return new SendOtpResponse
                {
                    Message = "Email cannot be empty",
                    IsSuccess = false
                };

            return await _apiService.PostAsync<SendOtpResponse>(EnvironmentsExtensions.ENDPOINT_SEND_MAIL_OTP, request);
        }
        catch (Exception ex)
        {
            return new SendOtpResponse
            {
                Message = ex.Message,
                IsSuccess = false
            };
        }
    }

    public async Task<LoginResponse?> VerifyEmailAsync(VerifyEmailRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Email.Trim()) || string.IsNullOrEmpty(request.Otp.Trim()))
                return new LoginResponse
                {
                    IsVerified = false
                };

            return await _apiService.PostAsync<LoginResponse>(EnvironmentsExtensions.ENDPOINT_VERIFY_AND_REGISTER_MAIL, request);
        }
        catch
        {
            return new LoginResponse
            {
                IsVerified = false
            };
        }
    }
}