using Application.Abstractions.Entities;

namespace Application.Abstractions.Cache;

public interface ICreatedReportRepository
{
    Task Add(ReportEntity report, CancellationToken cancellationToken);

    Task<ReportEntity?> Get(Guid reportId, CancellationToken cancellationToken);
}