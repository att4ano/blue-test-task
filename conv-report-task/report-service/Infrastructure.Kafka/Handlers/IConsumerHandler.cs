using Infrastructure.Kafka.Models;

namespace Infrastructure.Kafka.Handlers;

public interface IConsumerHandler<TKey, TValue>
{
    Task HandleAsync(
        IReadOnlyCollection<KafkaMessage<TKey, TValue>> messages,
        CancellationToken cancellationToken);
}