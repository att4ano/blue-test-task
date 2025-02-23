using Application.Abstractions;
using Application.Events;
using Google.Protobuf.WellKnownTypes;
using Infrastructure.Kafka.Models;
using Infrastructure.Kafka.Producer;
using Reports.Kafka.Contracts;

namespace Infrastructure.Kafka.Publisher;

public class EventPublisher : IEventPublisher
{
    private readonly KafkaOutboxProducer<ReportCreationKey, ReportCreationValue> _producer;

    public EventPublisher(KafkaOutboxProducer<ReportCreationKey, ReportCreationValue> producer)
    {
        _producer = producer;
    }

    public async Task ProduceCreationReportEvent(IEnumerable<CreationEvent> kafkaEvents, CancellationToken cancellationToken)
    {
        var messages = kafkaEvents.Select(ev => {
            var key = new ReportCreationKey
            {
                ReportId = new ReportCreationKey.Types.ReportId
                {
                    Guid = ev.ReportId.ToString(),
                },
            };
            
            var value = new ReportCreationValue
            {
                ProductId = ev.ProductId,
                StartPeriod = ev.StartPeriod.ToTimestamp(),
                EndPeriod = ev.EndPeriod.ToTimestamp(),
            };
            
            return new KafkaProducerMessage<ReportCreationKey, ReportCreationValue>(key, value);
        });
        
        await _producer.ProduceAsync(messages, cancellationToken);
    }
}