using Confluent.Kafka;
using Google.Protobuf;
using Infrastructure.Kafka.Consumers;
using Infrastructure.Kafka.Options;
using Infrastructure.Kafka.Serializers;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaOptions(this IServiceCollection collection)
    {
        collection.AddOptions<ConsumerOptions>().BindConfiguration("Kafka:Consumer");
        return collection;
    }

    public static IServiceCollection AddKafkaInfrastructureConsumer<TKey, TValue>(this IServiceCollection collection)
        where TKey : IMessage<TKey>, new()
        where TValue : IMessage<TValue>, new()
    {
        collection.AddSingleton<IDeserializer<TKey>, ProtoSerializer<TKey>>();
        collection.AddSingleton<IDeserializer<TValue>, ProtoSerializer<TValue>>();
        collection.AddScoped<IKafkaConsumer<TKey, TValue>, KafkaConsumer<TKey, TValue>>();
        
        return collection;
    }
}