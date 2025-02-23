namespace Infrastructure.Kafka.Models;

public record KafkaMessage<TKey, TValue>(TKey Key, TValue Value);