using System.Text.Json;
using Infrastructure.Kafka.Models;

namespace Infrastructure.Kafka.Mapper;

public static class MessageMapper
{
    public static KafkaProducerMessage<TKey, TValue> ToKafkaMessage<TKey, TValue>(this PersistenceKafkaMessage message)
    {
        var key = JsonSerializer.Deserialize<TKey>(message.Key);
        var value = JsonSerializer.Deserialize<TValue>(message.Value);

        if (key is null || value is null)
            throw new JsonException("cannot deserialize value");
        
        return new KafkaProducerMessage<TKey, TValue>(key, value);
    }
}