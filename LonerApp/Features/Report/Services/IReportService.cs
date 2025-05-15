namespace LonerApp.Services;

public interface IReportService
{
    Task<ReportResponse> ReportAsync(ReportRequest request);
    Task<CheckBlockedResponse> CheckBlockedAsync(CheckBlockedRequest request);
}