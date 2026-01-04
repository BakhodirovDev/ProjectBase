using System.Text.RegularExpressions;

namespace Application.Extensions;

/// <summary>
/// UserAgent stringdan qurilma va brauzer ma'lumotlarini ajratib olish uchun utility class
/// </summary>
public static class DeviceInfoExtractor
{
    /// <summary>
    /// UserAgent stringni tahlil qilib, qurilma nomi va turini qaytaradi
    /// </summary>
    /// <param name="userAgent">HTTP request UserAgent string</param>
    /// <returns>Qurilma nomi va turi</returns>
    public static (string DeviceName, string DeviceType) ParseUserAgent(string userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
            return ("Unknown Device", "Unknown");

        // OS, brauzer va qurilma turini aniqlash
        string os = DetectOperatingSystem(userAgent);
        string browser = DetectBrowser(userAgent);
        string deviceType = DetectDeviceType(userAgent);

        string deviceName = string.Empty;

        // Mobil yoki tablet bo'lsa, qurilma modelini aniqlashga harakat qilamiz
        if (deviceType == "Mobile" || deviceType == "Tablet")
        {
            deviceName = DetectMobileDevice(userAgent, os);
        }
        else // Desktop yoki boshqa qurilmalar uchun
        {
            deviceName = DetectDesktopDevice(userAgent, os, browser);
        }

        // Agar hali ham aniqlanmagan bo'lsa, default qiymat
        if (string.IsNullOrWhiteSpace(deviceName) || deviceName == "Unknown Device")
        {
            deviceName = $"{os} {browser}";
        }

        return (deviceName.Trim(), deviceType);
    }

    /// <summary>
    /// Operatsion tizimni aniqlash
    /// </summary>
    private static string DetectOperatingSystem(string userAgent)
    {
        string ua = userAgent.ToLowerInvariant();

        if (ua.Contains("windows nt") || ua.Contains("win64") || ua.Contains("win32"))
            return "Windows";
        if (ua.Contains("mac os x") || ua.Contains("macintosh"))
            return "macOS";
        if (ua.Contains("android"))
            return "Android";
        if (ua.Contains("iphone") || ua.Contains("ipad") || ua.Contains("ipod"))
            return "iOS";
        if (ua.Contains("linux") && !ua.Contains("android"))
            return "Linux";
        if (ua.Contains("cros"))
            return "Chrome OS";
        if (ua.Contains("blackberry") || ua.Contains("bb10"))
            return "BlackBerry";
        if (ua.Contains("windows phone"))
            return "Windows Phone";

        return "Unknown OS";
    }

    /// <summary>
    /// Brauzer turini aniqlash (tartib muhim!)
    /// </summary>
    private static string DetectBrowser(string userAgent)
    {
        string ua = userAgent.ToLowerInvariant();

        // Chromium-based brauzerlar birinchi tekshiriladi (Edge, Opera, Brave, etc.)
        if (ua.Contains("edg/") || ua.Contains("edga/") || ua.Contains("edgios/"))
            return "Edge";
        if (ua.Contains("opr/") || ua.Contains("opera"))
            return "Opera";
        if (ua.Contains("brave/") || ua.Contains("brave"))
            return "Brave";
        if (ua.Contains("vivaldi"))
            return "Vivaldi";
        if (ua.Contains("yabrowser"))
            return "Yandex Browser";
        if (ua.Contains("samsungbrowser"))
            return "Samsung Internet";
        if (ua.Contains("ucbrowser") || ua.Contains("ucweb"))
            return "UC Browser";
        if (ua.Contains("chrome/") || ua.Contains("crios/"))
            return "Chrome";
        if (ua.Contains("safari/") && !ua.Contains("chrome") && !ua.Contains("chromium"))
            return "Safari";
        if (ua.Contains("firefox/") || ua.Contains("fxios/"))
            return "Firefox";
        if (ua.Contains("trident/") || ua.Contains("msie"))
            return "Internet Explorer";
        if (ua.Contains("electron/"))
            return "Electron";

        return "Unknown Browser";
    }

    /// <summary>
    /// Qurilma turini aniqlash
    /// </summary>
    private static string DetectDeviceType(string userAgent)
    {
        string ua = userAgent.ToLowerInvariant();

        // Tablet birinchi tekshiriladi (chunki ba'zi tabletlarda "mobi" ham bo'lishi mumkin)
        if (ua.Contains("ipad"))
            return "Tablet";
        if (ua.Contains("tablet") || ua.Contains("tab"))
            return "Tablet";
        if (ua.Contains("android") && !ua.Contains("mobi") && !ua.Contains("mobile"))
            return "Tablet";

        // Mobil qurilmalar
        if (ua.Contains("mobi") || ua.Contains("mobile"))
            return "Mobile";
        if (ua.Contains("iphone") || ua.Contains("ipod"))
            return "Mobile";
        if (ua.Contains("android") && (ua.Contains("mobi") || ua.Contains("mobile")))
            return "Mobile";
        if (ua.Contains("blackberry") || ua.Contains("bb10") || ua.Contains("windows phone"))
            return "Mobile";

        // Desktop/Laptop
        if (ua.Contains("windows nt") || ua.Contains("macintosh") || ua.Contains("x11") ||
            ua.Contains("linux") || ua.Contains("cros"))
            return "Desktop";

        // Smart TV va boshqalar
        if (ua.Contains("smarttv") || ua.Contains("smart-tv") || ua.Contains("googletv"))
            return "Smart TV";
        if (ua.Contains("playstation") || ua.Contains("xbox") || ua.Contains("nintendo"))
            return "Gaming Console";

        return "Unknown";
    }

    /// <summary>
    /// Windows versiyasini aniqlash
    /// </summary>
    private static string DetectWindowsVersion(string userAgent)
    {
        var regex = new Regex(@"windows nt (\d+\.\d+)", RegexOptions.IgnoreCase);
        var match = regex.Match(userAgent.ToLowerInvariant());

        if (match.Success)
        {
            string version = match.Groups[1].Value;
            return version switch
            {
                "10.0" => "Windows 10/11",
                "6.3" => "Windows 8.1",
                "6.2" => "Windows 8",
                "6.1" => "Windows 7",
                "6.0" => "Windows Vista",
                "5.2" => "Windows XP x64",
                "5.1" => "Windows XP",
                _ => $"Windows (NT {version})"
            };
        }

        // 64-bit yoki 32-bit belgilari
        if (userAgent.Contains("Win64") || userAgent.Contains("x64"))
            return "Windows (64-bit)";
        if (userAgent.Contains("Win32"))
            return "Windows (32-bit)";

        return "Windows";
    }

    /// <summary>
    /// macOS versiyasini aniqlash
    /// </summary>
    private static string DetectMacOSVersion(string userAgent)
    {
        var regex = new Regex(@"mac os x (\d+)[_\.](\d+)[_\.]?(\d+)?", RegexOptions.IgnoreCase);
        var match = regex.Match(userAgent.ToLowerInvariant());

        if (match.Success)
        {
            string major = match.Groups[1].Value;
            string minor = match.Groups[2].Value;
            string patch = match.Groups.Count > 3 && !string.IsNullOrEmpty(match.Groups[3].Value)
                ? $".{match.Groups[3].Value}"
                : "";

            return $"macOS {major}.{minor}{patch}";
        }

        return "macOS";
    }

    /// <summary>
    /// iOS qurilmalarini aniqlash
    /// </summary>
    private static string DetectIOSDevice(string userAgent)
    {
        string ua = userAgent.ToLowerInvariant();
        string deviceType = "Unknown iOS Device";

        if (ua.Contains("iphone"))
            deviceType = "iPhone";
        else if (ua.Contains("ipad"))
            deviceType = "iPad";
        else if (ua.Contains("ipod"))
            deviceType = "iPod";

        // iOS versiyasini aniqlash
        var osVersionRegex = new Regex(@"(?:cpu )?(?:iphone )?os (\d+)[_\.]?(\d+)?[_\.]?(\d+)?", RegexOptions.IgnoreCase);
        var match = osVersionRegex.Match(ua);

        if (match.Success)
        {
            string major = match.Groups[1].Value;
            string minor = match.Groups.Count > 2 && !string.IsNullOrEmpty(match.Groups[2].Value)
                ? $".{match.Groups[2].Value}"
                : "";
            string patch = match.Groups.Count > 3 && !string.IsNullOrEmpty(match.Groups[3].Value)
                ? $".{match.Groups[3].Value}"
                : "";

            return $"{deviceType} (iOS {major}{minor}{patch})";
        }

        return deviceType;
    }

    /// <summary>
    /// Android qurilmalarini aniqlash
    /// </summary>
    private static string DetectAndroidDevice(string userAgent)
    {
        string ua = userAgent.ToLowerInvariant();

        // Android versiyasini aniqlash
        var versionRegex = new Regex(@"android (\d+\.?\d*\.?\d*)", RegexOptions.IgnoreCase);
        var versionMatch = versionRegex.Match(ua);
        string androidVersion = versionMatch.Success ? versionMatch.Groups[1].Value : "";

        // Qurilma modelini aniqlash uchun bir nechta pattern
        // Pattern 1: Standard format - Android X.X; Device Model Build/...
        var modelRegex1 = new Regex(@"android\s[\d\.]+;\s*(?:[^\);]+;\s*)?([^\);]+?)(?:\s+build|;|\))", RegexOptions.IgnoreCase);
        var match1 = modelRegex1.Match(ua);

        if (match1.Success)
        {
            string model = match1.Groups[1].Value.Trim();

            // Ba'zi umumiy formatlarni tozalash
            model = CleanAndroidModel(model);

            if (!string.IsNullOrWhiteSpace(model) &&
                !model.Equals("android", StringComparison.OrdinalIgnoreCase) &&
                !model.Equals("mobile", StringComparison.OrdinalIgnoreCase))
            {
                return !string.IsNullOrEmpty(androidVersion)
                    ? $"{model} (Android {androidVersion})"
                    : model;
            }
        }

        // Agar model aniqlanmasa, faqat Android versiyasini qaytarish
        return !string.IsNullOrEmpty(androidVersion)
            ? $"Android {androidVersion} Device"
            : "Android Device";
    }

    /// <summary>
    /// Android model nomini tozalash va formatlash
    /// </summary>
    private static string CleanAndroidModel(string model)
    {
        if (string.IsNullOrWhiteSpace(model))
            return model;

        // "Build" va undan keyingi qismlarni olib tashlash
        int buildIndex = model.IndexOf("build", StringComparison.OrdinalIgnoreCase);
        if (buildIndex > 0)
            model = model.Substring(0, buildIndex).Trim();

        // Ortiqcha bo'sh joylarni olib tashlash
        model = Regex.Replace(model, @"\s+", " ").Trim();

        // Ba'zi brendlarni formatlash
        model = FormatBrandName(model);

        return model;
    }

    /// <summary>
    /// Brend nomlarini to'g'ri formatlash
    /// </summary>
    private static string FormatBrandName(string model)
    {
        // Mashhur brendlar
        var brands = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"sm-", "Samsung "},
            {"gt-", "Samsung "},
            {"sch-", "Samsung "},
            {"sgh-", "Samsung "},
            {"lg-", "LG "},
            {"vs", "LG VS"},
            {"htc", "HTC"},
            {"moto", "Motorola"},
            {"pixel", "Google Pixel"},
            {"oneplus", "OnePlus"},
            {"mi ", "Xiaomi Mi "},
            {"redmi", "Xiaomi Redmi"},
            {"pocophone", "Pocophone"},
            {"poco", "POCO"},
            {"oppo", "OPPO"},
            {"vivo", "Vivo"},
            {"realme", "Realme"},
            {"huawei", "Huawei"},
            {"honor", "Honor"},
            {"asus", "ASUS"},
            {"nokia", "Nokia"},
            {"lenovo", "Lenovo"}
        };

        foreach (var brand in brands)
        {
            if (model.StartsWith(brand.Key, StringComparison.OrdinalIgnoreCase))
            {
                return brand.Value + model.Substring(brand.Key.Length).Trim();
            }
        }

        return model;
    }

    /// <summary>
    /// Mobil qurilmalarni aniqlash (iOS va Android)
    /// </summary>
    private static string DetectMobileDevice(string userAgent, string os)
    {
        if (os.Equals("iOS", StringComparison.OrdinalIgnoreCase))
        {
            return DetectIOSDevice(userAgent);
        }
        else if (os.Equals("Android", StringComparison.OrdinalIgnoreCase))
        {
            return DetectAndroidDevice(userAgent);
        }
        else if (os.Equals("Windows Phone", StringComparison.OrdinalIgnoreCase))
        {
            return "Windows Phone Device";
        }
        else if (os.Equals("BlackBerry", StringComparison.OrdinalIgnoreCase))
        {
            return "BlackBerry Device";
        }

        return "Mobile Device";
    }

    /// <summary>
    /// Desktop qurilmalarni aniqlash
    /// </summary>
    private static string DetectDesktopDevice(string userAgent, string os, string browser)
    {
        if (os.Equals("Windows", StringComparison.OrdinalIgnoreCase))
        {
            string winVersion = DetectWindowsVersion(userAgent);
            return $"{winVersion} - {browser}";
        }
        else if (os.Equals("macOS", StringComparison.OrdinalIgnoreCase))
        {
            string macVersion = DetectMacOSVersion(userAgent);
            return $"{macVersion} - {browser}";
        }
        else if (os.Equals("Linux", StringComparison.OrdinalIgnoreCase))
        {
            // Linux distributivini aniqlash
            string distro = DetectLinuxDistro(userAgent);
            return $"{distro} - {browser}";
        }
        else if (os.Equals("Chrome OS", StringComparison.OrdinalIgnoreCase))
        {
            return $"Chromebook - {browser}";
        }

        return $"{os} - {browser}";
    }

    /// <summary>
    /// Linux distributivini aniqlash
    /// </summary>
    private static string DetectLinuxDistro(string userAgent)
    {
        string ua = userAgent.ToLowerInvariant();

        if (ua.Contains("ubuntu"))
            return "Ubuntu Linux";
        if (ua.Contains("fedora"))
            return "Fedora Linux";
        if (ua.Contains("debian"))
            return "Debian Linux";
        if (ua.Contains("mint"))
            return "Linux Mint";
        if (ua.Contains("arch"))
            return "Arch Linux";
        if (ua.Contains("manjaro"))
            return "Manjaro Linux";
        if (ua.Contains("red hat") || ua.Contains("rhel"))
            return "Red Hat Linux";
        if (ua.Contains("suse"))
            return "SUSE Linux";
        if (ua.Contains("centos"))
            return "CentOS";

        return "Linux";
    }

    /// <summary>
    /// Brauzer versiyasini aniqlash
    /// </summary>
    /// <param name="userAgent">UserAgent string</param>
    /// <param name="browser">Brauzer nomi</param>
    /// <returns>Brauzer versiyasi</returns>
    public static string GetBrowserVersion(string userAgent, string browser)
    {
        if (string.IsNullOrWhiteSpace(userAgent) || string.IsNullOrWhiteSpace(browser))
            return "";

        string ua = userAgent.ToLowerInvariant();
        string browserKey = browser.ToLowerInvariant().Replace(" ", "");

        // Har bir brauzer uchun regex pattern
        var patterns = new Dictionary<string, string>
        {
            {"chrome", @"chrome\/(\d+\.?\d*\.?\d*\.?\d*)"},
            {"firefox", @"firefox\/(\d+\.?\d*\.?\d*)"},
            {"safari", @"version\/(\d+\.?\d*\.?\d*)"},
            {"edge", @"edg\/(\d+\.?\d*\.?\d*\.?\d*)"},
            {"opera", @"opr\/(\d+\.?\d*\.?\d*\.?\d*)"},
            {"internetexplorer", @"(?:msie |rv:)(\d+\.?\d*)"},
            {"brave", @"brave\/(\d+\.?\d*\.?\d*)"},
            {"vivaldi", @"vivaldi\/(\d+\.?\d*\.?\d*)"},
            {"yandexbrowser", @"yabrowser\/(\d+\.?\d*\.?\d*)"},
            {"samsunginternet", @"samsungbrowser\/(\d+\.?\d*\.?\d*)"},
            {"ucbrowser", @"ucbrowser\/(\d+\.?\d*\.?\d*)"}
        };

        if (patterns.TryGetValue(browserKey, out string pattern))
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var match = regex.Match(ua);

            if (match.Success)
                return match.Groups[1].Value;
        }

        return "";
    }

    /// <summary>
    /// To'liq qurilma ma'lumotini DTO formatida qaytarish
    /// </summary>
    public static DeviceInfoDto GetDetailedDeviceInfo(string userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
            return new DeviceInfoDto();

        var (deviceName, deviceType) = ParseUserAgent(userAgent);
        string os = DetectOperatingSystem(userAgent);
        string browser = DetectBrowser(userAgent);
        string browserVersion = GetBrowserVersion(userAgent, browser);

        return new DeviceInfoDto
        {
            DeviceName = deviceName,
            DeviceType = deviceType,
            OperatingSystem = os,
            Browser = browser,
            BrowserVersion = browserVersion,
            IsBot = IsBot(userAgent),
            IsMobile = deviceType == "Mobile",
            IsTablet = deviceType == "Tablet",
            IsDesktop = deviceType == "Desktop",
            UserAgent = userAgent
        };
    }

    /// <summary>
    /// Bot/Crawler ekanligini aniqlash
    /// </summary>
    private static bool IsBot(string userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
            return false;

        string ua = userAgent.ToLowerInvariant();

        string[] botKeywords =
        {
            "bot", "crawler", "spider", "scraper", "curl", "wget",
            "googlebot", "bingbot", "slurp", "duckduckbot", "baiduspider",
            "yandexbot", "facebookexternalhit", "whatsapp", "telegram",
            "ia_archiver", "archive.org_bot", "semrushbot", "ahrefsbot"
        };

        return botKeywords.Any(keyword => ua.Contains(keyword));
    }
}

/// <summary>
/// Qurilma ma'lumotlari uchun DTO class
/// </summary>
public class DeviceInfoDto
{
    public string DeviceName { get; set; } = "Unknown Device";
    public string DeviceType { get; set; } = "Unknown";
    public string OperatingSystem { get; set; } = "Unknown OS";
    public string Browser { get; set; } = "Unknown Browser";
    public string BrowserVersion { get; set; } = "";
    public bool IsBot { get; set; }
    public bool IsMobile { get; set; }
    public bool IsTablet { get; set; }
    public bool IsDesktop { get; set; }
    public string UserAgent { get; set; } = "";
}