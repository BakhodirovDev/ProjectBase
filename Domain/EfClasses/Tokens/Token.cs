using Domain.Abstraction.Base;

namespace Domain.EfClasses;

public class Token : AuditableEntity
{
    public Token() : base()
    {
    }

    public Guid UserId { get; set; }
    public Guid AccessTokenId { get; set; }
    public DateTime AccessTokenExpireAt { get; set; }
    public Guid RefreshTokenId { get; set; }
    public DateTime RefreshTokenExpireAt { get; set; }

    // Navigation Propertys
    public virtual User User { get; set; } = null!;
    public virtual DeviceInfo DeviceInfo { get; set; } = null!;

    public void UpdateAccessToken(Guid accessTokenId, DateTime accessTokenExpiryTime)
    {
        AccessTokenId = accessTokenId;
        AccessTokenExpireAt = accessTokenExpiryTime;
    }

    public void UpdateRefreshToken(Guid refreshTokenId, DateTime refreshTokenExpiryTime)
    {
        RefreshTokenId = refreshTokenId;
        RefreshTokenExpireAt = refreshTokenExpiryTime;
    }
}