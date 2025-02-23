namespace Application.Models;

public record Report(
    long ProductId,
    DateTime StartPeriod,
    DateTime EndPeriod,
    double Ratio,
    int PaymentCount,
    int ViewCount);