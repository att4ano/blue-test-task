using System.Threading.Channels;
using Infrastructure.Kafka.Models;
using Infrastructure.Kafka.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.Kafka.Handlers;

public class ConsumerMessageHandler<TKey, TValue>
{
    private readonly IConsumerHandler<TKey, TValue> _handler;
    private readonly int _time;
    private readonly int _batchSize;

    public ConsumerMessageHandler(IConsumerHandler<TKey, TValue> handler, IOptions<ConsumerOptions> options)
    {
        _handler = handler;
        _time = options.Value.Timeout;
        _batchSize = options.Value.BatchSize;
    }

    public async Task Handle(
        ChannelReader<KafkaMessage<TKey, TValue>> reader,
        CancellationToken cancellationToken)
    {
        await Task.Yield();

        IAsyncEnumerable<IReadOnlyList<KafkaMessage<TKey, TValue>>> messageChunks =
            reader.ReadAllAsync(cancellationToken).ChunkAsync(_batchSize, TimeSpan.FromSeconds(_time));

        await foreach (IReadOnlyList<KafkaMessage<TKey, TValue>> messages in messageChunks)
        {
            await _handler.HandleAsync(messages, cancellationToken);
        }
    }
}