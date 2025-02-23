using System.Threading.Channels;
using Confluent.Kafka;
using Infrastructure.Kafka.Mapper;
using Infrastructure.Kafka.Models;
using Infrastructure.Kafka.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.Kafka.Consumers;

public class KafkaConsumer<TKey, TValue> : IKafkaConsumer<TKey, TValue>
{
    private readonly IConsumer<TKey, TValue> _consumer;

    public KafkaConsumer(
        IOptions<ConsumerOptions> consumerOptions,
        IDeserializer<TKey> keyDeserializer,
        IDeserializer<TValue> valueDeserializer)
    {
        _consumer = new ConsumerBuilder<TKey, TValue>(new ConsumerConfig
            {
                BootstrapServers = consumerOptions.Value.Host,
                EnableAutoOffsetStore = true,
                GroupId = consumerOptions.Value.GroupId,
            })
            .SetKeyDeserializer(keyDeserializer)
            .SetValueDeserializer(valueDeserializer)
            .Build();

        _consumer.Subscribe(consumerOptions.Value.Topic);
    }

    public async Task Consume(ChannelWriter<KafkaMessage<TKey, TValue>> writer, CancellationToken cancellationToken)
    {
        await Task.Yield();

        while (cancellationToken.IsCancellationRequested is false)
        {
            var result = _consumer.Consume(cancellationToken);
            var message = result.ToMessage();
            await writer.WriteAsync(message, cancellationToken: cancellationToken);
        }

        writer.Complete();
    }

    public void Dispose()
    {
        _consumer.Close();
    }
}