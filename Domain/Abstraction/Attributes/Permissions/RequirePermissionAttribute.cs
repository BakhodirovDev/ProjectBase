using Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permission)
    {
        Policy = $"Permission:{permission}";
    }

    public RequirePermissionAttribute(object permission)
    {
        if (permission == null)
            throw new ArgumentNullException(nameof(permission));

        if (!permission.GetType().IsEnum)
            throw new ArgumentException("Parameter must be an enum value", nameof(permission));

        var enumValue = (Enum)permission;
        string permissionString = PermissionHelper.GetPermissionString(enumValue);
        Policy = $"Permission:{permissionString}";
    }
}