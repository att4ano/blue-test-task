namespace Presentation.Dto.Request;

public record CreateReportRequest(DateTime StartPeriod, DateTime EndPeriod, long ProductId);