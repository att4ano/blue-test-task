namespace Application.Abstractions.Models;

public record InsertReportModel(
    Guid Id,
    long ProductId,
    DateTime StartPeriod,
    DateTime EndPeriod,
    double Ratio,
    int PaymentCount,
    int ViewCount);