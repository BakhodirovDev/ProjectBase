using Domain.Abstraction;
using Domain.Abstraction.Base;

namespace Domain.EfClasses;

public class DeviceInfo : AuditableEntity
{
    public DeviceInfo() : base()
    {
    }

    /// <summary>
    /// Foydalanuvchining IP manzili (IPv4 yoki IPv6)
    /// </summary>
    public string IpAddress { get; set; } = null!;

    /// <summary>
    /// Qurilma turi (Mobile, Tablet, Desktop, Smart TV, Gaming Console, Unknown)
    /// </summary>
    public string DeviceType { get; set; } = null!;

    /// <summary>
    /// Qurilma modeli yoki nomi
    /// Misol: "iPhone (iOS 17.2)", "Windows 10/11 - Chrome", "Samsung SM-S918B"
    /// </summary>
    public string DeviceModel { get; set; } = null!;

    /// <summary>
    /// Operatsion tizim nomi (Windows, macOS, iOS, Android, Linux, etc.)
    /// </summary>
    public string OsName { get; set; } = null!;

    /// <summary>
    /// Operatsion tizim versiyasi
    /// Misol: "10.0", "17.2", "13", "10.15.7"
    /// </summary>
    public string OsVersion { get; set; } = null!;

    /// <summary>
    /// Brauzer nomi (Chrome, Firefox, Safari, Edge, etc.)
    /// </summary>
    public string BrowserName { get; set; } = null!;

    /// <summary>
    /// Brauzer versiyasi
    /// Misol: "120.0.0.0", "121.0", "17.1"
    /// </summary>
    public string BrowserVersion { get; set; } = null!;

    /// <summary>
    /// To'liq UserAgent string (logging va debugging uchun)
    /// </summary>
    public string UserAgent { get; set; } = null!;

    /// <summary>
    /// Bu bot yoki crawler mi?
    /// </summary>
    public bool IsBot { get; set; }

    /// <summary>
    /// Mobil qurilma mi?
    /// </summary>
    public bool IsMobile { get; set; }

    /// <summary>
    /// Tablet mi?
    /// </summary>
    public bool IsTablet { get; set; }

    /// <summary>
    /// Desktop mi?
    /// </summary>
    public bool IsDesktop { get; set; }

    /// <summary>
    /// Geografik joylashuv (Country/City) - ixtiyoriy
    /// Misol: "Uzbekistan, Tashkent"
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Mamlakat kodi (ISO 3166-1 alpha-2) - ixtiyoriy
    /// Misol: "UZ", "US", "RU"
    /// </summary>
    public string? CountryCode { get; set; }

    /// <summary>
    /// Ushbu qurilmadan oxirgi faollik vaqti
    /// </summary>
    public DateTime? LastActivityAt { get; set; }

    /// <summary>
    /// Ushbu qurilmadan necha marta login bo'lgan
    /// </summary>
    public int LoginCount { get; set; }

    /// <summary>
    /// Qurilma trust qilinganmi (masalan, biometrik autentifikatsiya orqali)
    /// </summary>
    public bool IsTrusted { get; set; }

    /// <summary>
    /// Qurilma nickname (foydalanuvchi o'zi qo'ygan nom)
    /// Misol: "Ish telefoni", "Uyda kompyuter"
    /// </summary>
    public string? DeviceNickname { get; set; }

    public Guid TokenId { get; set; }

    // Navigation properties
    public virtual Token Token { get; set; } = null!;

    public void UpdateGeolocation(IpLocationInfo locationInfo)
    {
        if (locationInfo == null) return;
        Location = $"{locationInfo.City}, {locationInfo.CountryName}";
        CountryCode = locationInfo.CountryCode;
    }

    public void UpdateActivity()
    {
        LastActivityAt = DateTime.UtcNow;
        LoginCount += 1;
    }
}