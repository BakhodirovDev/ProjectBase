namespace Domain.Abstraction.Options.RateLimitOptions;

public class ConcurrencyLimiterOptions
{
    public int PermitLimit { get; set; }
    public int QueueLimit { get; set; }
}