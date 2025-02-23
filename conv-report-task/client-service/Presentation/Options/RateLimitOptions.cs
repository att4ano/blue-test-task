namespace Presentation.Options;

public class RateLimitOptions
{
    public int PermitLimit { get; set; }
    
    public int WindowSeconds { get; set; }
    
    public int QueueLimit { get; set; }
}