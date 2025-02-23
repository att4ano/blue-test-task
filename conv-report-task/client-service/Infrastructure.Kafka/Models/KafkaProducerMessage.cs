namespace Infrastructure.Kafka.Models;

public record KafkaProducerMessage<TKey, TValue>(TKey Key, TValue Value);