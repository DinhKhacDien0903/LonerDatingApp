namespace LonerApp.Features.Filter.Services
{
    public interface IFilterService
    {
        Task<UpdateLocationResponse> UpdateLocationAsync(UpdateLocationRequest request);
        Task<GetMemberByLocationAndRadiusResponse> GetMemberByLocationAndRadiusAsync(GetMemberByLocationAndRadiusRequest request);
    }
}