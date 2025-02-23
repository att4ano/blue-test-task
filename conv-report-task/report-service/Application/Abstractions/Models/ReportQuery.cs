namespace Application.Abstractions.Models;

public record ReportQuery(IReadOnlyCollection<Guid> Ids);