using Application.Abstractions;
using Confluent.Kafka;
using Google.Protobuf;
using Infrastructure.Kafka.Handlers;
using Infrastructure.Kafka.MessagePersistence;
using Infrastructure.Kafka.Options;
using Infrastructure.Kafka.Producer;
using Infrastructure.Kafka.Publisher;
using Infrastructure.Kafka.Serializers;
using Infrastructure.Kafka.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataOptions(this IServiceCollection collection)
    {
        collection.AddOptions<PostgresOptions>().BindConfiguration("DataAccess:Postgres");
        return collection;
    }
    
    public static IServiceCollection AddKafkaOptions(this IServiceCollection collection)
    {
        collection.AddOptions<ProducerOptions>().BindConfiguration("Kafka:Producer");
        return collection;
    }
    
    public static IServiceCollection AddKafkaProducer<TKey, TValue>(this IServiceCollection collection)
        where TKey : IMessage<TKey>, new()
        where TValue : IMessage<TValue>, new()
    {
        collection.AddSingleton<ISerializer<TValue>, ProtoSerializer<TValue>>();
        collection.AddSingleton<ISerializer<TKey>, ProtoSerializer<TKey>>();
        collection.AddScoped<IKafkaMessageProducer<TKey, TValue>, KafkaProducer<TKey, TValue>>();
        collection.AddScoped<IMessagePersistenceHandler<TKey, TValue>, MessagePersistenceHandler<TKey, TValue>>();
        collection.AddHostedService<MessagePersistenceBackgroundService<TKey, TValue>>();
        return collection;
    }

    public static IServiceCollection AddOutbox<TKey, TValue>(this IServiceCollection collection)
    {
        collection.AddScoped<KafkaOutboxProducer<TKey, TValue>>();
        collection.AddScoped<IMessagePersistenceRepository, MessagePersistenceRepository>();
        return collection;
    }
    
    public static IServiceCollection AddEventPublisher(this IServiceCollection collection)
    {
        collection.AddScoped<IEventPublisher, EventPublisher>();
        return collection;
    }
}