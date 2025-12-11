namespace Domain.Abstraction.Options.RateLimitOptions;

public class FixedWindowOptions
{
    public int PermitLimit { get; set; }
    public int WindowInMinutes { get; set; }
    public int QueueLimit { get; set; }
}