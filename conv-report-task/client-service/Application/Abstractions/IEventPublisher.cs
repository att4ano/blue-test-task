using Application.Events;

namespace Application.Abstractions;

public interface IEventPublisher
{
    Task ProduceCreationReportEvent(IEnumerable<CreationEvent> kafkaEvents, CancellationToken cancellationToken);
}