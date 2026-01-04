using System.ComponentModel;
using System.Reflection;

namespace Helpers;

public static class PermissionHelper
{
    public static string GetPermissionString<TEnum>(TEnum permission) where TEnum : Enum
    {
        string resource = GetResourceName(typeof(TEnum));
        string action = permission.ToString().ToLower();

        return $"{resource}.{action}";
    }

    public static string GetPermissionString(Enum permission)
    {
        string resource = GetResourceName(permission.GetType());
        string action = permission.ToString().ToLower();

        return $"{resource}.{action}";
    }

    public static string GetDescription<TEnum>(TEnum value) where TEnum : Enum
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }

    public static string GetDescription(Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }

    private static string GetResourceName(Type enumType)
    {
        string typeName = enumType.Name;
        if (typeName.EndsWith("Permissions"))
        {
            return typeName.Substring(0, typeName.Length - 11).ToLower();
        }
        return typeName.ToLower();
    }

    public static IEnumerable<string> GetAllPermissions<TEnum>() where TEnum : Enum
    {
        return Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Select(GetPermissionString);
    }

    public static Dictionary<string, string> GetAllPermissionsWithDescriptions<TEnum>() where TEnum : Enum
    {
        return Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .ToDictionary(
                permission => GetPermissionString(permission),
                permission => GetDescription(permission)
            );
    }
}