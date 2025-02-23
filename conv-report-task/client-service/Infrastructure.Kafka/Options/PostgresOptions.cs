namespace Infrastructure.Kafka.Options;

public class PostgresOptions
{
    public string Host { get; set; } = string.Empty;
    
    public int Port { get; set; }

    public string Password { get; set; } = string.Empty;
    
    public string Username { get; set; } = string.Empty;
}