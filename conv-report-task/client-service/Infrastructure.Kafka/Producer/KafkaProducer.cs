using Infrastructure.Kafka.Models;
using Infrastructure.Kafka.Options;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Kafka.Producer;

public class KafkaProducer<TKey, TValue> : IKafkaMessageProducer<TKey, TValue>
{
    private readonly IProducer<TKey, TValue> _producer;
    private readonly ILogger<KafkaProducer<TKey, TValue>> _logger;
    private readonly string _topicName;

    public KafkaProducer(
        IOptions<ProducerOptions> options,
        ISerializer<TValue> valueSerializer,
        ISerializer<TKey> keySerializer,
        ILogger<KafkaProducer<TKey, TValue>> logger)
    {
        _logger = logger;
        _topicName = options.Value.Topic;
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = options.Value.Host,
        };

        _producer = new ProducerBuilder<TKey, TValue>(producerConfig)
            .SetKeySerializer(keySerializer)
            .SetValueSerializer(valueSerializer)
            .Build();
    }

    public async Task ProduceAsync(
        IEnumerable<KafkaProducerMessage<TKey, TValue>> messages,
        CancellationToken cancellationToken)
    {
        try
        {
            foreach (KafkaProducerMessage<TKey, TValue> message in messages)
            {
                var producerMessage = new Message<TKey, TValue>
                {
                    Key = message.Key,
                    Value = message.Value,
                };
                await _producer.ProduceAsync(_topicName, producerMessage, cancellationToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }
}