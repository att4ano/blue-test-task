using System.Runtime.CompilerServices;
using Application.Models;

namespace Application.Contracts;

public interface IReportService
{
    Task CreateReport(CreateReportDto reportDto, CancellationToken cancellationToken);

    Task<Report> CheckReport(Guid reportIds, CancellationToken cancellationToken);
}