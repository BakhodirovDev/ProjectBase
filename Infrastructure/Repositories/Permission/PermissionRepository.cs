using Domain.EfClasses.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class PermissionRepository : Repository<Permission>, IPermissionRepository
{
    public PermissionRepository(DbContext context, ILogger<Repository<Permission, Guid>> logger) : base(context, logger)
    {
    }

    public async Task<List<Permission>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<RolePermission>()
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.Permission)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Permission>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserRole>()
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission)
            .Distinct()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task<List<Permission>> GetByResourceAsync(string resource, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Resource.ToLower() == resource.ToLower())
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> UserHasPermissionAsync(Guid userId, string permissionName, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserRole>()
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.RolePermissions)
            .AnyAsync(rp => rp.Permission.Name.ToLower() == permissionName.ToLower(), cancellationToken);
    }
}