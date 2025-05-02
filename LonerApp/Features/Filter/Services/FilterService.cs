namespace LonerApp.Features.Filter.Services
{
    public class FilterService : IFilterService
    {
        private readonly IApiService _apiService;
        public FilterService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<GetMemberByLocationAndRadiusResponse> GetMemberByLocationAndRadiusAsync(GetMemberByLocationAndRadiusRequest request)
        {
            try
            {
                //?UserId=ds&Longitude=6&Latitude=6&Radius=6
                //var query = $"?UserId={UserId}&Longitude={Longitude}&Latitude={Latitude}&Radius={Radius}" ;
                var response = await _apiService.PostAsync<GetMemberByLocationAndRadiusResponse>(EnvironmentsExtensions.ENDPOINT_GET_BY_LOCATION_RADIUS, request);
                return response;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error during when get member by location && radius: {ex.Message}", ex);
                return null;
            }
        }

        public async Task<UpdateLocationResponse> UpdateLocationAsync(UpdateLocationRequest request)
        {
            try
            {
                var response = await _apiService.PostAsync<UpdateLocationResponse>(EnvironmentsExtensions.ENDPOINT_UPDATE_LOCATION, request);
                return response;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error during when update location: {ex.Message}", ex);
                return null;
            }
        }
    }
}