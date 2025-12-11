using Microsoft.AspNetCore.Http;
using System.Net;

namespace Application.Extensions;

public static class HttpContextExtensions
{
    /// <summary>
    /// Client IP manzilini olish uchun kengaytma metod.
    /// Proxy, load balancer va CDN'larni hisobga oladi.
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <returns>Client IP manzili</returns>
    public static string GetClientIpAddress(this HttpContext context)
    {
        // 1. X-Forwarded-For sarlavhasini tekshirish
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            var ipAddresses = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                          .Select(ip => ip.Trim());

            foreach (var ip in ipAddresses)
            {
                if (IsValidPublicIpAddress(ip))
                {
                    return ip;
                }
            }
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(realIp) && IsValidPublicIpAddress(realIp))
        {
            return realIp;
        }

        var cloudflareIp = context.Request.Headers["CF-Connecting-IP"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(cloudflareIp) && IsValidPublicIpAddress(cloudflareIp))
        {
            return cloudflareIp;
        }

        var clientIp = context.Request.Headers["X-Client-IP"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(clientIp) && IsValidPublicIpAddress(clientIp))
        {
            return clientIp;
        }

        return context.Connection.RemoteIpAddress.ToString();
    }

    /// <summary>
    /// IP manzilining to'g'ri formatda va public ekanligini tekshiradi.
    /// </summary>
    /// <param name="ipAddress">IP manzili</param>
    /// <returns>True - agar IP to'g'ri va public bo'lsa</returns>
    private static bool IsValidPublicIpAddress(string ipAddress)
    {
        if (!IPAddress.TryParse(ipAddress, out var parsedIp))
        {
            return false;
        }

        // Private va loopback IP manzillarni filtrlash
        var bytes = parsedIp.GetAddressBytes();

        // IPv4 uchun
        if (parsedIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        {
            // Loopback: 127.0.0.0/8
            if (bytes[0] == 127)
                return false;

            // Private: 10.0.0.0/8
            if (bytes[0] == 10)
                return false;

            // Private: 172.16.0.0/12
            if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
                return false;

            // Private: 192.168.0.0/16
            if (bytes[0] == 192 && bytes[1] == 168)
                return false;

            // Link-local: 169.254.0.0/16
            if (bytes[0] == 169 && bytes[1] == 254)
                return false;
        }

        // IPv6 uchun
        if (parsedIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            // Loopback: ::1
            if (IPAddress.IsLoopback(parsedIp))
                return false;

            // Link-local: fe80::/10
            if (bytes[0] == 0xfe && (bytes[1] & 0xc0) == 0x80)
                return false;

            // Unique local: fc00::/7
            if ((bytes[0] & 0xfe) == 0xfc)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Barcha IP sarlavhalarini olish
    /// </summary>
    public static Dictionary<string, string?> GetAllIpHeaders(this HttpContext context)
    {
        return new Dictionary<string, string?>
        {
            ["X-Forwarded-For"] = context.Request.Headers["X-Forwarded-For"].FirstOrDefault(),
            ["X-Real-IP"] = context.Request.Headers["X-Real-IP"].FirstOrDefault(),
            ["CF-Connecting-IP"] = context.Request.Headers["CF-Connecting-IP"].FirstOrDefault(),
            ["X-Client-IP"] = context.Request.Headers["X-Client-IP"].FirstOrDefault(),
            ["RemoteIpAddress"] = context.Connection.RemoteIpAddress?.ToString()
        };
    }
}