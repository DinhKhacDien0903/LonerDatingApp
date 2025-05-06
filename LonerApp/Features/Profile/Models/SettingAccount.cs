namespace LonerApp.Models;

public class GetSettingAccountResponse
{
    public SettingAccountResponse SettingAccount { get; set; } = new();
}
public class SettingAccountResponse
{
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public bool ShowGender { get; set; }
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
}
