using Confluent.Kafka;
using Infrastructure.Kafka.Models;

namespace Infrastructure.Kafka.Mapper;

public static class MessageMapper
{
    public static KafkaMessage<TKey, TValue> ToMessage<TKey, TValue>(this ConsumeResult<TKey, TValue> result)
    {
        return new KafkaMessage<TKey, TValue>(
            result.Message.Key,
            result.Message.Value);
    }
}