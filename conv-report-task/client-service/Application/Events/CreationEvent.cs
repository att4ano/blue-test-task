namespace Application.Events;

public record CreationEvent(DateTime StartPeriod, DateTime EndPeriod, long ProductId, Guid ReportId);