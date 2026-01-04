using Consts;

namespace Domain.EfClasses;

public class GenerateTokenDto
{
    public User User { get; set; } = null!;
    public bool IsUpdateToken { get; set; }
    public Guid AccessTokenId { get; set; } = Guid.NewGuid();
    public Guid RefreshTokenId { get; set; } = Guid.NewGuid();
}