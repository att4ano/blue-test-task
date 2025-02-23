using System.Text.Json;
using Infrastructure.Kafka.MessagePersistence;
using Infrastructure.Kafka.Models;
using Infrastructure.Kafka.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.Kafka.Producer;

public class KafkaOutboxProducer<TKey, TValue>
{
    private readonly IMessagePersistenceRepository _messagePersistenceRepository;
    private readonly string _topic;

    public KafkaOutboxProducer(
        IOptions<ProducerOptions> options,
        IMessagePersistenceRepository messagePersistenceRepository)
    {
        _messagePersistenceRepository = messagePersistenceRepository;
        _topic = options.Value.Topic;
    }
    
    public async Task ProduceAsync(IEnumerable<KafkaProducerMessage<TKey, TValue>> messages, CancellationToken cancellationToken)
    {
        var persistedMessages = messages.Select(message => new PersistenceKafkaMessage
        {
            Id = default,
            TopicName = _topic,
            CreatedAt = DateTimeOffset.Now,
            State = MessageState.Pending,
            Key = JsonSerializer.Serialize(message.Key),
            Value = JsonSerializer.Serialize(message.Value),
        }).ToArray();

        await _messagePersistenceRepository.AddAsync(persistedMessages, cancellationToken);
    }
}