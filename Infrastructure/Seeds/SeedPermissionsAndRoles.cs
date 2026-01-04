using Attributes;
using Domain.EfClasses.Authentication;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Reflection;

namespace Infrastructure.Seeds;

/// <summary>
/// Permission va Role ma'lumotlarini database'ga seed qilish
/// </summary>
public static class SeedPermissionsAndRoles
{
    #region Constants

    /// <summary>
    /// Super Admin role uchun statik ID
    /// </summary>
    private static readonly Guid SuperAdminRoleId = Guid.Parse("b99ca8ea-d24f-4845-977b-7c53ffa4b2e4");

    /// <summary>
    /// Super Admin User uchun statik ID
    /// </summary>
    private static readonly Guid SuperAdminUserId = Guid.Parse("41896b5b-1eb8-401d-a8d6-bb7d0fc5792d");

    /// <summary>
    /// Super Admin role nomi
    /// </summary>
    private const string SuperAdminRoleName = "SuperAdmin";

    /// <summary>
    /// Super Admin role tavsifi
    /// </summary>
    private const string SuperAdminRoleDescription = "Super Administrator with all permissions";

    #endregion

    #region Public Methods

    /// <summary>
    /// Barcha permission va role'larni seed qilish
    /// </summary>
    public static async Task SeedAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
        var dbContext = services.GetRequiredService<ApplicationDbContext>();

        logger.LogInformation("Starting permissions and roles seeding...");

        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            var (permissionsAdded, permissionsUpdated) = await SeedPermissionsAsync(dbContext, logger);
            var rolesAdded = await SeedRolesAsync(dbContext, logger);
            await EnsureSuperAdminHasAllPermissionsAsync(dbContext, logger);
            await AssignSuperAdminRoleToUserAsync(dbContext, logger);

            await transaction.CommitAsync();

            logger.LogInformation(
                "Seeding completed successfully. Permissions added: {PermissionsAdded}, Permissions updated: {PermissionsUpdated}, Roles added: {RolesCount}",
                permissionsAdded, permissionsUpdated, rolesAdded);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError(ex, "An error occurred while seeding permissions and roles. Transaction rolled back.");
            throw;
        }
    }

    #endregion

    #region Permission Seeding

    /// <summary>
    /// Permission enum'lardan permission'larni database'ga seed qilish va yangilash
    /// </summary>
    private static async Task<(int Added, int Updated)> SeedPermissionsAsync(ApplicationDbContext dbContext, ILogger logger)
    {
        var existingPermissions = await dbContext.Set<Permission>()
            .ToDictionaryAsync(p => p.Name, p => p);

        var permissionEnumTypes = GetPermissionEnumTypes();

        if (!permissionEnumTypes.Any())
        {
            logger.LogWarning("No permission enum types found. Make sure permission enums have [PermissionModule] attribute.");
            return (0, 0);
        }

        var permissionsToAdd = new List<Permission>();
        var permissionsToUpdate = new List<Permission>();

        foreach (var enumType in permissionEnumTypes)
        {
            var moduleInfo = GetModuleInfo(enumType);

            foreach (var enumValue in Enum.GetValues(enumType))
            {
                var fieldInfo = enumType.GetField(enumValue.ToString()!);
                if (fieldInfo == null) continue;

                var permissionInfo = fieldInfo.GetCustomAttribute<PermissionInfoAttribute>();
                var description = fieldInfo.GetCustomAttribute<DescriptionAttribute>()?.Description
                                  ?? enumValue.ToString()!;

                string permissionName = permissionInfo?.Code
                                        ?? $"{moduleInfo.ModuleName.ToLowerInvariant()}.{enumValue.ToString()!.ToLowerInvariant()}";

                string resource = moduleInfo.ModuleName.ToLowerInvariant();
                string action = enumValue.ToString()!.ToLowerInvariant();

                // Mavjud permission'ni tekshirish
                if (existingPermissions.TryGetValue(permissionName, out var existingPermission))
                {
                    bool needsUpdate = false;

                    if (existingPermission.Description != description)
                    {
                        existingPermission.Description = description;
                        needsUpdate = true;
                    }

                    if (existingPermission.Resource != resource)
                    {
                        existingPermission.Resource = resource;
                        needsUpdate = true;
                    }

                    if (existingPermission.Action != action)
                    {
                        existingPermission.Action = action;
                        needsUpdate = true;
                    }

                    if (needsUpdate)
                    {
                        permissionsToUpdate.Add(existingPermission);
                    }
                }
                else
                {
                    var permission = new Permission
                    {
                        Name = permissionName,
                        Description = description,
                        Resource = resource,
                        Action = action
                    };

                    permissionsToAdd.Add(permission);
                }
            }
        }

        int addedCount = 0;
        int updatedCount = 0;

        if (permissionsToAdd.Count > 0)
        {
            await dbContext.Set<Permission>().AddRangeAsync(permissionsToAdd);
            await dbContext.SaveChangesAsync();
            addedCount = permissionsToAdd.Count;

            logger.LogInformation(
                "Added {Count} new permissions: {Permissions}",
                addedCount,
                string.Join(", ", permissionsToAdd.Select(p => p.Name)));
        }

        if (permissionsToUpdate.Count > 0)
        {
            await dbContext.SaveChangesAsync();
            updatedCount = permissionsToUpdate.Count;

            logger.LogInformation(
                "Updated {Count} permissions: {Permissions}",
                updatedCount,
                string.Join(", ", permissionsToUpdate.Select(p => p.Name)));
        }

        if (addedCount == 0 && updatedCount == 0)
        {
            logger.LogDebug("No permissions to add or update.");
        }

        return (addedCount, updatedCount);
    }

    /// <summary>
    /// [PermissionModule] attribute'ga ega barcha enum'larni topish
    /// </summary>
    private static List<Type> GetPermissionEnumTypes()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch (ReflectionTypeLoadException)
                {
                    return Array.Empty<Type>();
                }
            })
            .Where(t => t.IsEnum && t.GetCustomAttribute<PermissionModuleAttribute>() != null)
            .ToList();
    }

    /// <summary>
    /// Enum tipidan modul ma'lumotlarini olish
    /// </summary>
    private static (string ModuleName, string DisplayName, string? Category) GetModuleInfo(Type enumType)
    {
        var moduleAttr = enumType.GetCustomAttribute<PermissionModuleAttribute>();
        return (
            moduleAttr?.ModuleName ?? GetDefaultModuleName(enumType.Name),
            moduleAttr?.DisplayName ?? enumType.Name,
            moduleAttr?.Category
        );
    }

    /// <summary>
    /// Generic property setter
    /// </summary>
    private static void SetProperty<T>(T entity, string propertyName, object value) where T : class
    {
        var property = typeof(T).GetProperty(propertyName);
        if (property != null && property.CanWrite)
        {
            property.SetValue(entity, value);
        }
    }

    /// <summary>
    /// Enum nomidan default modul nomini olish
    /// </summary>
    private static string GetDefaultModuleName(string enumTypeName)
    {
        if (enumTypeName.EndsWith("Permissions", StringComparison.OrdinalIgnoreCase))
        {
            return enumTypeName[..^11];
        }

        if (enumTypeName.EndsWith("Permission", StringComparison.OrdinalIgnoreCase))
        {
            return enumTypeName[..^10];
        }

        return enumTypeName;
    }

    #endregion

    #region Role Seeding

    /// <summary>
    /// Role'larni database'ga seed qilish
    /// </summary>
    private static async Task<int> SeedRolesAsync(ApplicationDbContext dbContext, ILogger logger)
    {
        var superAdminExists = await dbContext.Set<Role>()
            .AsNoTracking()
            .AnyAsync(r => r.Id == SuperAdminRoleId);

        if (superAdminExists)
        {
            logger.LogDebug("SuperAdmin role already exists.");
            return 0;
        }

        var superAdminRole = CreateRole(
            SuperAdminRoleId,
            SuperAdminRoleName,
            SuperAdminRoleDescription,
            isSystemAdmin: true,
            isSystemRole: true
        );

        await dbContext.Set<Role>().AddAsync(superAdminRole);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Created SuperAdmin role with ID: {RoleId}", SuperAdminRoleId);

        await AssignAllPermissionsToRoleAsync(dbContext, SuperAdminRoleId, logger);

        return 1;
    }

    /// <summary>
    /// Role entity yaratish (private constructor bilan)
    /// </summary>
    private static Role CreateRole(Guid id, string name, string description, bool isSystemAdmin, bool isSystemRole)
    {
        // Reflection orqali private constructor chaqirish
        var role = (Role)Activator.CreateInstance(
            typeof(Role),
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            Array.Empty<object>(),
            null)!;

        role.IdSet(id);

        SetProperty(role, nameof(Role.Name), name);
        SetProperty(role, nameof(Role.Description), description);
        SetProperty(role, nameof(Role.IsSystemAdmin), isSystemAdmin);
        SetProperty(role, nameof(Role.IsSystemRole), isSystemRole);

        return role;
    }

    /// <summary>
    /// Barcha permission'larni berilgan role'ga biriktirish
    /// </summary>
    private static async Task AssignAllPermissionsToRoleAsync(
        ApplicationDbContext dbContext,
        Guid roleId,
        ILogger logger)
    {
        var allPermissionIds = await dbContext.Set<Permission>()
            .AsNoTracking()
            .Select(p => p.Id)
            .ToListAsync();

        var existingRolePermissionIds = await dbContext.Set<RolePermission>()
            .AsNoTracking()
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.PermissionId)
            .ToHashSetAsync();

        var rolePermissionsToAdd = allPermissionIds
            .Where(permissionId => !existingRolePermissionIds.Contains(permissionId))
            .Select(permissionId => new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId
            })
            .ToList();

        if (rolePermissionsToAdd.Count > 0)
        {
            await dbContext.Set<RolePermission>().AddRangeAsync(rolePermissionsToAdd);
            await dbContext.SaveChangesAsync();

            logger.LogInformation(
                "Assigned {Count} permissions to role {RoleId}",
                rolePermissionsToAdd.Count, roleId);
        }
    }

    #endregion

    #region SuperAdmin User Assignment

    /// <summary>
    /// SuperAdmin role'ga barcha yangi permission'larni qo'shish
    /// </summary>
    private static async Task EnsureSuperAdminHasAllPermissionsAsync(
        ApplicationDbContext dbContext,
        ILogger logger)
    {
        var superAdminRoleExists = await dbContext.Set<Role>()
            .AsNoTracking()
            .AnyAsync(r => r.Id == SuperAdminRoleId);

        if (!superAdminRoleExists)
        {
            logger.LogWarning("SuperAdmin role not found. Skipping permission assignment.");
            return;
        }

        await AssignAllPermissionsToRoleAsync(dbContext, SuperAdminRoleId, logger);
    }

    /// <summary>
    /// SuperAdmin role'ni belgilangan foydalanuvchiga biriktirish
    /// </summary>
    private static async Task AssignSuperAdminRoleToUserAsync(
        ApplicationDbContext dbContext,
        ILogger logger)
    {
        var userRoleExists = await dbContext.Set<UserRole>()
            .AsNoTracking()
            .AnyAsync(ur => ur.UserId == SuperAdminUserId && ur.RoleId == SuperAdminRoleId);

        if (userRoleExists)
        {
            logger.LogDebug("SuperAdmin User already has SuperAdmin role.");
            return;
        }

        var superAdminRoleExists = await dbContext.Set<Role>()
            .AsNoTracking()
            .AnyAsync(r => r.Id == SuperAdminRoleId);

        if (!superAdminRoleExists)
        {
            logger.LogWarning("SuperAdmin role not found. Cannot assign role to User.");
            return;
        }

        var userRole = new UserRole
        {
            UserId = SuperAdminUserId,
            RoleId = SuperAdminRoleId
        };

        await dbContext.Set<UserRole>().AddAsync(userRole);
        await dbContext.SaveChangesAsync();

        logger.LogInformation(
            "Assigned SuperAdmin role to User {UserId}",
            SuperAdminUserId);
    }

    #endregion
}