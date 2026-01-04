namespace Permissions;

/// <summary>
/// Permission to'liq ma'lumotlari uchun model
/// </summary>
public class PermissionDetails
{
    public string Permission { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCritical { get; set; }
    public bool IsReadOnly { get; set; }
    public string[]? DependsOn { get; set; }
    public string? ModuleName { get; set; }
    public string? ModuleDisplayName { get; set; }
    public string? Category { get; set; }
    public int Priority { get; set; }
}
