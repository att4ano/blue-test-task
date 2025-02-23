using System.Runtime.CompilerServices;
using Application.Abstractions;
using Application.Contracts;
using Application.Events;
using Application.Mapper;
using Reports.Service;
using Report = Application.Models.Report;

namespace Application.Application;

public class ReportService : IReportService
{
    private readonly IEventPublisher _publisher;
    private readonly Reports.Service.ReportService.ReportServiceClient _client;

    public ReportService(Reports.Service.ReportService.ReportServiceClient client, IEventPublisher publisher)
    {
        _client = client;
        _publisher = publisher;
    }

    public async Task<Guid> CreateReport(DateTime startPeriod, DateTime endPeriod, long productId,
        CancellationToken cancellationToken)
    {
        Guid guid = Guid.NewGuid();
        var creationEvent = new CreationEvent(startPeriod, endPeriod, productId, guid);
        await _publisher.ProduceCreationReportEvent([creationEvent], cancellationToken);

        return guid;
    }

    public async Task<Report> CheckReport(Guid reportId, CancellationToken cancellationToken)
    {
        var request = new GetReportRequest
        {
            Id = new ReportId
            {
                Guid = reportId.ToString(),
            },
        };

        var response = await _client.GetReportAsync(request, cancellationToken: cancellationToken);
        return response.Report.ToModel();
    }
}