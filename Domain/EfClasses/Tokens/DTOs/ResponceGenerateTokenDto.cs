namespace Domain.EfClasses;

public class ResponceGenerateTokenDto
{
    public string AccessToken { get; set; } = null!;
    public DateTime AccessTokenExpiryTime { get; set; }
    public string RefreshToken { get; set; } = null!;
    public DateTime RefreshTokenExpiryTime { get; set; }
    public string AuthType { get; set; } = null!;
}