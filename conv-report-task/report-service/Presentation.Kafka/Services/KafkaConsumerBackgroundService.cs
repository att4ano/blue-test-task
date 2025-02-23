using System.Threading.Channels;
using Infrastructure.Kafka.Consumers;
using Infrastructure.Kafka.Handlers;
using Infrastructure.Kafka.Models;
using Infrastructure.Kafka.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Presentation.Kafka.Services;

public class KafkaConsumerBackgroundService<TKey, TValue> : BackgroundService
{
    private readonly IServiceScopeFactory _factory;
    private readonly int _channelSize;

    public KafkaConsumerBackgroundService(
        IOptions<ConsumerOptions> options,
        IServiceScopeFactory factory)
    {
        _factory = factory;
        _channelSize = options.Value.BatchSize;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = _factory.CreateScope();
        var channel = Channel.CreateBounded<KafkaMessage<TKey, TValue>>(_channelSize);

        using var consumer = scope.ServiceProvider
            .GetRequiredService<IKafkaConsumer<TKey, TValue>>();
        var handler = scope.ServiceProvider
            .GetRequiredService<ConsumerMessageHandler<TKey, TValue>>();

        var consume = consumer.Consume(channel.Writer, stoppingToken);
        var handle = handler.Handle(channel.Reader, stoppingToken);

        await Task.WhenAll(consume, handle);
    }
}