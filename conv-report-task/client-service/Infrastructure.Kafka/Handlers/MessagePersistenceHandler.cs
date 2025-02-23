using Infrastructure.Kafka.Models;
using Infrastructure.Kafka.Producer;

namespace Infrastructure.Kafka.Handlers;

public class MessagePersistenceHandler<TKey, TValue> : IMessagePersistenceHandler<TKey, TValue>
{
    private readonly IKafkaMessageProducer<TKey, TValue> _producer;

    public MessagePersistenceHandler(IKafkaMessageProducer<TKey, TValue> producer)
    {
        _producer = producer;
    }

    public async Task HandleAsync(IEnumerable<KafkaProducerMessage<TKey, TValue>> messages, CancellationToken cancellationToken)
    {
        var producerMessages = messages
            .Select(x => new KafkaProducerMessage<TKey, TValue>(x.Key, x.Value)).ToArray();

        await _producer.ProduceAsync(producerMessages, cancellationToken);
    }
}