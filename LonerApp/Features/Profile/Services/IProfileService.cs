namespace LonerApp.Features.Services;

public interface IProfileService
{
    Task<GetProfileDetailResponse?> GetProfileDetailAsync(string endpoint, string UserId);
    Task<GetSettingAccountResponse?> GetSettingAccountAsync( string UserId);
    Task<UpdateUserInforResponse?> UpdateUserInforAsync(UpdateUserInforRequest request);
}