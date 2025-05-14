namespace LonerApp.Models;

public record ReportRequest
{
    public ReportRequestDto Request { get; set; } = new ReportRequestDto();
}

public class ReportRequestDto
{
    public string ReporterId { get; set; } = string.Empty;
    public string ReportedId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string MoreInformation { get; set; } = string.Empty;
    public byte TypeBlocked { get; set; } // 0: block profile, 1: block chat, 2: report
}
public record ReportResponse(bool IsSuccess);