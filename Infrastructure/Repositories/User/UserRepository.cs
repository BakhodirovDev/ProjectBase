using Domain.Abstraction.Errors;
using Domain.Abstraction.Results;
using Domain.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(DbContext context, ILogger<Repository<User, Guid>> logger) : base(context, logger)
    {
    }

    public async Task<Result<User>> GetByUsername(string username, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await GetQueryable()
                .FirstOrDefaultAsync(
                    u => u.UserName.ToLower() == username.ToLower(),
                    cancellationToken);

            return user is not null
                ? Result<User>.Success(user)
                : Result<User>.Failure(Error.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting User by username: {Username}", username);
            throw;
        }
    }
}