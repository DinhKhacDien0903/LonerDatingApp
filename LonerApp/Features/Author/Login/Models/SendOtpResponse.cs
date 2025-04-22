namespace LonerApp.Features.Author.Login.Models;

public class SendOtpResponse
{
    public string Message { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
}