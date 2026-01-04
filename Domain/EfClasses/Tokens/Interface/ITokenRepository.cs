using Domain.Abstraction.Base;
using Domain.Abstraction.Results;

namespace Domain.EfClasses;

public interface ITokenRepository : IBaseRepository<Token>
{
    Task<Result<Token>> FindByUserId(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<Token>> GetByAccessTokenId(Guid accessTokenId, CancellationToken cancellationToken = default);
    Task<Result<Token>> GetByRefreshTokenId(Guid refreshTokenId, CancellationToken cancellationToken = default);
}