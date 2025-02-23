using System.Transactions;
using Infrastructure.Kafka.Extensions;
using Infrastructure.Kafka.Handlers;
using Infrastructure.Kafka.Mapper;
using Infrastructure.Kafka.MessagePersistence;
using Infrastructure.Kafka.Models;
using Infrastructure.Kafka.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Kafka.Services;

public class MessagePersistenceBackgroundService<TKey, TValue> : BackgroundService
{
    private readonly string _topicName;
    private readonly IServiceScopeFactory _factory;
    private readonly ILogger<MessagePersistenceBackgroundService<TKey, TValue>> _logger;

    public MessagePersistenceBackgroundService(
        IServiceScopeFactory factory,
        ILogger<MessagePersistenceBackgroundService<TKey, TValue>> logger,
        IOptions<ProducerOptions> options)
    {
        _factory = factory;
        _logger = logger;
        _topicName = options.Value.Topic;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ExecuteMigrationsAsync(stoppingToken);
        
        await using var scope = _factory.CreateAsyncScope();
        while (stoppingToken.IsCancellationRequested is false)
        {
            var repository = scope.ServiceProvider.GetRequiredService<IMessagePersistenceRepository>();
            using var transaction = CreateTransactionScope();
            var messages = await repository
                .QueryAllPending(_topicName, stoppingToken)
                .ToArrayAsync(cancellationToken: stoppingToken);

            if (messages is [])
            {
                transaction.Complete();
                continue;
            }

            var handler = scope.ServiceProvider.GetRequiredService<IMessagePersistenceHandler<TKey, TValue>>();
            var kafkaMessages = messages.Select(message => message.ToKafkaMessage<TKey, TValue>());

            try
            {
                await handler.HandleAsync(kafkaMessages, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error during handling messages: {e.Message}");
                throw;
            }

            messages = messages.Select(message =>
            {
                message.State = MessageState.Completed;
                return message;
            }).ToArray();

            await repository.UpdateAsync(messages, stoppingToken);
            transaction.Complete();
        }
    }

    private async Task ExecuteMigrationsAsync(CancellationToken stoppingToken)
    {
        using var scope = _factory.CreateScope();
        scope.ServiceProvider.DoMigrations();
        await Task.CompletedTask;
    }
    
    private static TransactionScope CreateTransactionScope(
        IsolationLevel level = IsolationLevel.ReadCommitted)
    {
        return new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions
            {
                IsolationLevel = level,
                Timeout = TimeSpan.FromSeconds(5),
            },
            TransactionScopeAsyncFlowOption.Enabled);
    }
}