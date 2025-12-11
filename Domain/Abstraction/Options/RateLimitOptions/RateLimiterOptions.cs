namespace Domain.Abstraction.Options.RateLimitOptions;

public class RateLimiterOptions
{
    public GlobalLimiterOptions GlobalLimiter { get; set; } = null!;
}