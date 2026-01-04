using Permissions;
using System.ComponentModel;
using System.Reflection;

namespace Attributes;

public static class PermissionAttributeHelper
{
    /// <summary>
    /// Permission haqida to'liq ma'lumot olish
    /// </summary>
    /// <typeparam name="TEnum">Permission enum tipi</typeparam>
    /// <param name="permission">Permission qiymati</param>
    public static PermissionDetails? GetPermissionDetails<TEnum>(TEnum permission)
        where TEnum : struct, Enum
    {
        var type = typeof(TEnum);

        if (!type.IsEnum)
            throw new ArgumentException($"{type.Name} must be an enum type");

        var memberInfo = type.GetMember(permission.ToString()!);
        if (memberInfo.Length == 0) return null;

        // PermissionInfo attribute ni o'qish
        var permissionInfo = memberInfo[0].GetCustomAttribute<PermissionInfoAttribute>();
        var description = memberInfo[0].GetCustomAttribute<DescriptionAttribute>();

        // Module attribute ni o'qish
        var moduleAttr = type.GetCustomAttribute<PermissionModuleAttribute>();

        return new PermissionDetails
        {
            Permission = permission.ToString()!,
            Code = permissionInfo?.Code ?? string.Empty,
            Description = description?.Description ?? string.Empty,
            IsCritical = permissionInfo?.IsCritical ?? false,
            IsReadOnly = permissionInfo?.IsReadOnly ?? false,
            DependsOn = permissionInfo?.DependsOn,
            ModuleName = moduleAttr?.ModuleName,
            ModuleDisplayName = moduleAttr?.DisplayName,
            Category = moduleAttr?.Category,
            Priority = moduleAttr?.Priority ?? 0
        };
    }

    /// <summary>
    /// Barcha permission ma'lumotlarini olish
    /// </summary>
    public static List<PermissionDetails> GetAllPermissions<TEnum>()
        where TEnum : struct, Enum
    {
        var permissions = Enum.GetValues<TEnum>();
        var result = new List<PermissionDetails>();

        foreach (var permission in permissions)
        {
            var details = GetPermissionDetails(permission);
            if (details != null)
            {
                result.Add(details);
            }
        }

        return result;
    }

    /// <summary>
    /// Kritik permissionlarni olish
    /// </summary>
    public static List<PermissionDetails> GetCriticalPermissions<TEnum>()
        where TEnum : struct, Enum
    {
        return GetAllPermissions<TEnum>()
            .Where(p => p.IsCritical)
            .ToList();
    }

    /// <summary>
    /// Permission tekshirish - bog'liqliklarni ham tekshiradi (Generic)
    /// </summary>
    public static bool CanExecute<TEnum>(TEnum permission, HashSet<string> userPermissions)
        where TEnum : struct, Enum
    {
        var details = GetPermissionDetails(permission);

        if (details == null) return false;

        if (!userPermissions.Contains(details.Code))
            return false;

        if (details.DependsOn != null)
        {
            foreach (var dependency in details.DependsOn)
            {
                if (!userPermissions.Contains(dependency))
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Permission code ni enum ga aylantirish
    /// </summary>
    public static TEnum? GetPermissionByCode<TEnum>(string code)
        where TEnum : struct, Enum
    {
        var permissions = Enum.GetValues<TEnum>();

        foreach (var permission in permissions)
        {
            var details = GetPermissionDetails(permission);
            if (details?.Code == code)
            {
                return permission;
            }
        }

        return null;
    }
}