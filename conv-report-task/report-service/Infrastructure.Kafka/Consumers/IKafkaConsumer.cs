using System.Threading.Channels;
using Infrastructure.Kafka.Models;

namespace Infrastructure.Kafka.Consumers;

public interface IKafkaConsumer<TKey, TValue> : IDisposable
{
    Task Consume(ChannelWriter<KafkaMessage<TKey, TValue>> writer, CancellationToken cancellationToken);
}