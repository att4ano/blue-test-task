namespace Application.Models;

public record CreateReportDto(DateTime StartPeriod, DateTime EndPeriod, long ProductId, Guid ReportId);