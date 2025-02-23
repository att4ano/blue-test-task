using Confluent.Kafka;
using Infrastructure.Kafka.Models;

namespace Infrastructure.Kafka.Handlers;

public interface IMessagePersistenceHandler<TKey, TValue>
{
    Task HandleAsync(IEnumerable<KafkaProducerMessage<TKey, TValue>> messages, CancellationToken cancellationToken);
}