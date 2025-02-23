using Infrastructure.Kafka.Models;

namespace Infrastructure.Kafka.Producer;

public interface IKafkaMessageProducer<TKey, TValue>
{
    Task ProduceAsync(
        IEnumerable<KafkaProducerMessage<TKey, TValue>> messages,
        CancellationToken cancellationToken);
}