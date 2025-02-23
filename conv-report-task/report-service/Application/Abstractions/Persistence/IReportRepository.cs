using Application.Abstractions.Entities;
using Application.Abstractions.Models;

namespace Application.Abstractions.Persistence;

public interface IReportRepository
{
    IAsyncEnumerable<ReportEntity> QueryAsync(ReportQuery query, CancellationToken cancellationToken);
    
    Task AddAsync(IReadOnlyCollection<InsertReportModel> reports, CancellationToken cancellationToken);
}