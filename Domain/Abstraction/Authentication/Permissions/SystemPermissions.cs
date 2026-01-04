using Attributes;
using System.ComponentModel;

namespace Permissions;

/// <summary>
/// System sozlamalari uchun ruxsatlar
/// </summary>
[PermissionModule("System", "Tizim sozlamalari")]
public enum SystemPermissions
{
    /// <summary>
    /// Tizim sozlamalarini ko'rish huquqi
    /// </summary>
    [Description("View system settings")]
    [PermissionInfo("system.view", IsReadOnly = true)]
    View,

    /// <summary>
    /// Tizim sozlamalarini o'zgartirish huquqi
    /// </summary>
    [Description("Update system settings")]
    [PermissionInfo("system.update", IsCritical = true)]
    Update,

    /// <summary>
    /// Tizim sozlamalarini export qilish huquqi
    /// </summary>
    [Description("Export system settings")]
    [PermissionInfo("system.export")]
    Export,

    /// <summary>
    /// Tizim sozlamalarini import qilish huquqi
    /// </summary>
    [Description("Import system settings")]
    [PermissionInfo("system.import", IsCritical = true)]
    Import,

    /// <summary>
    /// Tizim loglarini ko'rish huquqi
    /// </summary>
    [Description("View system logs")]
    [PermissionInfo("system.viewLogs")]
    ViewLogs,

    /// <summary>
    /// Tizim backup yaratish huquqi
    /// </summary>
    [Description("Create system backup")]
    [PermissionInfo("system.backup", IsCritical = true)]
    Backup,

    /// <summary>
    /// Tizimni restore qilish huquqi
    /// </summary>
    [Description("Restore system")]
    [PermissionInfo("system.restore", IsCritical = true)]
    Restore,
}
