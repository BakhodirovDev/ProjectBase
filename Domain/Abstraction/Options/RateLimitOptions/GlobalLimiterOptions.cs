namespace Domain.Abstraction.Options.RateLimitOptions;

public class GlobalLimiterOptions
{
    public int PermitLimit { get; set; }
    public double WindowInMinutes { get; set; }
    public int QueueLimit { get; set; }
}
