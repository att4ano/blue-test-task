namespace Infrastructure.Kafka.Options;

public class ProducerOptions
{
    public string Host { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
}