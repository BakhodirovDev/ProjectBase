namespace Domain.Abstraction.Jwt;

public class JwtOption
{
    public string SecretKey { get; set; } = null!;
    public string RefreshTokenSecretKey { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int? ExpiresInMinutes { get; set; }
    public int? RefreshTokenExpiresInDays { get; set; }
}