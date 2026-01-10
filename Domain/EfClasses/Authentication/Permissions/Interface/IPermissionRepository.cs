using Domain.Abstraction.Base;

namespace Domain.EfClasses.Authentication;

public interface IPermissionRepository : IBaseRepository<Permission>
{
    /// <summary>
    /// Role ID bo'yicha permissionlarni olish
    /// </summary>
    Task<List<Permission>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// User ID bo'yicha permissionlarni olish
    /// </summary>
    Task<List<Permission>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Permission nomini bo'yicha olish
    /// </summary>
    Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resource bo'yicha permissionlarni olish
    /// </summary>
    Task<List<Permission>> GetByResourceAsync(string resource, CancellationToken cancellationToken = default);

    /// <summary>
    /// Foydalanuvchida permission borligini tekshirish
    /// </summary>
    Task<bool> UserHasPermissionAsync(Guid userId, string permissionName, CancellationToken cancellationToken = default);
}