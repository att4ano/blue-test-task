namespace Application.Abstractions.Models;

public record OrderQuery(
    IReadOnlyCollection<long> Ids,
    IReadOnlyCollection<long> ProductIds,
    DateTime? StartPeriod,
    DateTime? EndPeriod,
    decimal? TotalPrice);