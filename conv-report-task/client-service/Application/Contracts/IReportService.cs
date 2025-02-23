using Application.Models;

namespace Application.Contracts;

public interface IReportService
{
    Task<Guid> CreateReport(DateTime startPeriod, DateTime endPeriod, long productId, CancellationToken cancellationToken);
    
    Task<Report> CheckReport(Guid reportId, CancellationToken cancellationToken);
}