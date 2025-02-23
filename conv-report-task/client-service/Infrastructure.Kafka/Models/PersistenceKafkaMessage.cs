namespace Infrastructure.Kafka.Models;

public record PersistenceKafkaMessage
{
    public long Id { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public MessageState State { get; set; }
}