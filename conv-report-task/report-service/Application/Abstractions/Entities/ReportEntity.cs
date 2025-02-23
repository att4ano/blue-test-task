namespace Application.Abstractions.Entities;

public record ReportEntity(
    Guid Id,
    long ProductId,
    DateTime StartPeriod,
    DateTime EndPeriod,
    double Ratio,
    int PaymentCount,
    int ViewCount);