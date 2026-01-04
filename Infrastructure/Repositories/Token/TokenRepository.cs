using Domain.Abstraction.Errors;
using Domain.Abstraction.Results;
using Domain.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Infrastructure.Repositories;

public class TokenRepository : Repository<Token>, ITokenRepository
{
    public TokenRepository(DbContext context, ILogger<Repository<Token, Guid>> logger) : base(context, logger)
    {
    }

    public async Task<Result<Token>> FindByUserId(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await GetQueryable()
                .FirstOrDefaultAsync(
                    u => u.UserId == userId);

            return token is not null
                ? Result<Token>.Success(token)
                : Result<Token>.Failure(Error.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Token by userId: {userId}", userId);
            throw;
        }
    }

    public async Task<Result<Token>> GetByAccessTokenId(Guid accessTokenId, CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await GetQueryable()
                .FirstOrDefaultAsync(
                    u => u.AccessTokenId == accessTokenId);

            return token is not null
                ? Result<Token>.Success(token)
                : Result<Token>.Failure(Error.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Token by accessTokenId: {accessTokenId}", accessTokenId);
            throw;
        }
    }

    public async Task<Result<Token>> GetByRefreshTokenId(Guid refreshTokenId, CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await GetQueryable()
                .FirstOrDefaultAsync(
                    u => u.RefreshTokenId == refreshTokenId);

            return token is not null
                ? Result<Token>.Success(token)
                : Result<Token>.Failure(Error.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Token by refreshTokenId: {refreshTokenId}", refreshTokenId);
            throw;
        }
    }
}