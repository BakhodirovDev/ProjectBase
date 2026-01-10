using Domain.Abstraction.Base;
using Domain.Abstraction.Results;

namespace Domain.EfClasses.Authentication;

public interface IPermissionService : ICrudService<PermissionDto, PermissionDto, CreatePermissionDto, UpdatePermissionDto>
{
    Task<Result<List<PermissionDto>>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default);

    Task<Result<List<PermissionDto>>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    Task<Result<List<PermissionDto>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result<PermissionDto>> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<Result<List<PermissionDto>>> GetByResourceAsync(string resource, CancellationToken cancellationToken = default);

    Task<Result<bool>> UserHasPermissionAsync(Guid userId, string permissionName, CancellationToken cancellationToken = default);

    Task<Result<bool>> UserHasAllPermissionsAsync(Guid userId, IEnumerable<string> permissionNames, CancellationToken cancellationToken = default);

    Task<Result<bool>> UserHasAnyPermissionAsync(Guid userId, IEnumerable<string> permissionNames, CancellationToken cancellationToken = default);
}