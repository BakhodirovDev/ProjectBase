namespace Attributes;

/// <summary>
/// Permission qiymati uchun qo'shimcha ma'lumotlar
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class PermissionInfoAttribute : Attribute
{
    /// <summary>
    /// Unique kod/identifier (masalan: "system.view", "User.create")
    /// Database da saqlash yoki API da ishlatish uchun
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Bu kritik amal mi? (xavfli operatsiya)
    /// Agar true bo'lsa:
    /// - Audit log yozilishi kerak
    /// - Qo'shimcha tasdiqlash so'ralishi mumkin
    /// - Email/SMS notification yuborilishi mumkin
    /// Masalan: Delete, Update kritik bo'lishi mumkin
    /// </summary>
    public bool IsCritical { get; set; }

    /// <summary>
    /// Bu faqat o'qish huquqi mi?
    /// Agar true bo'lsa, faqat ma'lumot ko'rish mumkin, o'zgartirish mumkin emas
    /// Masalan: View, List kabi operatsiyalar
    /// </summary>
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// Bu permission boshqa permissionlarga bog'liq
    /// Masalan: User.Delete uchun User.View kerak bo'lishi mumkin
    /// </summary>
    /// <example>
    /// DependsOn = new[] { "User.View", "User.Update" }
    /// </example>
    public string[]? DependsOn { get; set; }

    public PermissionInfoAttribute(string code)
    {
        Code = code;
    }
}