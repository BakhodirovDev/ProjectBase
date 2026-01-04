namespace Attributes;

/// <summary>
/// Permission moduli uchun metadata
/// </summary>
[AttributeUsage(AttributeTargets.Enum)]
public class PermissionModuleAttribute : Attribute
{
    /// <summary>
    /// Modul nomi (masalan: "System", "User", "Branch")
    /// </summary>
    public string ModuleName { get; }

    // <summary>
    /// Ko'rsatiladigan nom (UI uchun)
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Kategoriya (masalan: "Administration", "Finance", "Education")
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Tartib raqami (UI da qaysi tartibda ko'rsatish)
    /// </summary>
    public int Priority { get; set; }

    public PermissionModuleAttribute(string moduleName, string displayName)
    {
        ModuleName = moduleName;
        DisplayName = displayName;
    }
}
