namespace Application.Abstractions.Models;

public record ViewQuery(
    IReadOnlyCollection<long> Ids,
    IReadOnlyCollection<long> ProductIds,
    DateTime? StartPeriod,
    DateTime? EndPeriod);