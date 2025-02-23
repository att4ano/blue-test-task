using Application.Contracts;
using Application.Models;
using Infrastructure.Kafka.Handlers;
using Infrastructure.Kafka.Models;
using Reports.Kafka.Contracts;

namespace Presentation.Kafka.Handlers;

public class ReportCreationHandler : IConsumerHandler<ReportCreationKey, ReportCreationValue>
{
    private readonly IReportService _reportService;

    public ReportCreationHandler(IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task HandleAsync(IReadOnlyCollection<KafkaMessage<ReportCreationKey, ReportCreationValue>> messages, CancellationToken cancellationToken)
    {
        foreach (var message in messages)
        {
            var createReport = new CreateReportDto(
                message.Value.StartPeriod.ToDateTime(), 
                message.Value.EndPeriod.ToDateTime(),
                message.Value.ProductId,
                Guid.Parse(message.Key.ReportId.Guid));

            await _reportService.CreateReport(createReport, cancellationToken: cancellationToken);
        }
    }
}