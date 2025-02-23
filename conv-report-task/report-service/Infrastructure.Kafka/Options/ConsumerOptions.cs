namespace Infrastructure.Kafka.Options;

public class ConsumerOptions
{
    public string Host { get; set; } = string.Empty;
    
    public string Topic { get; set; } = string.Empty;

    public string GroupId { get; set; } = string.Empty;
    
    public int Timeout { get; set; }
    
    public int BatchSize { get; set; }
    
    public int ChannelCapacity { get; set; }
}