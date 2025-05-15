namespace LonerApp.Services;

public class ReportService : IReportService
{
    private readonly IApiService _apiService;
    public ReportService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<CheckBlockedResponse> CheckBlockedAsync(CheckBlockedRequest request)
    {
        try
        {
            var response = await _apiService.PostAsync<CheckBlockedResponse>(EnvironmentsExtensions.ENDPOINT_CHECK_BLOCKED_CHAT, request);
            return response;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error during swipe operation: {ex.Message}", ex);
            return new CheckBlockedResponse(false);
        }
    }

    public async Task<ReportResponse> ReportAsync(ReportRequest request)
    {
        try
        {
            var response = await _apiService.PostAsync<ReportResponse>(EnvironmentsExtensions.ENDPOINT_REPORT_USER_PROFILE, request);
            return response;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error during swipe operation: {ex.Message}", ex);
            return new ReportResponse(false);
        }
    }
}