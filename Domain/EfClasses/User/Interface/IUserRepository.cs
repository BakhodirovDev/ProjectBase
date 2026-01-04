using Domain.Abstraction.Base;
using Domain.Abstraction.Results;

namespace Domain.EfClasses;

public interface IUserRepository : IBaseRepository<User>
{
    Task<Result<User>> GetByUsername(string username, CancellationToken cancellationToken = default);
}