using Microsoft.AspNetCore.Http;

namespace Application.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Client IP manzilini olish uchun kengaytma metod.
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns>Client IP manzili</returns>
        public static string GetClientIpAddress(this HttpContext context)
        {
            // X-Forwarded-For sarlavhasini olish (proksi orqali kelgan IP manzillar)
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            // Agar X-Forwarded-For sarlavhasi mavjud bo'lsa
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                // IP manzillarni vergul bilan ajratib, birinchi haqiqiy IP manzilini qaytaradi
                var ipAddresses = forwardedFor.Split(',').Select(ip => ip.Trim()).ToList();

                // IP manzillar ro'yxatidagi birinchi ishonchli IPni qaytarish
                // (agar bir nechta IP manzillari bo'lsa, birinchi haqiqiy IPni tanlash)
                foreach (var ip in ipAddresses)
                {
                    if (IsValidIpAddress(ip))
                    {
                        return ip;
                    }
                }
            }

            // Agar X-Forwarded-For sarlavhasi mavjud bo'lmasa yoki ishonchli IP topilmasa
            return context.Connection.RemoteIpAddress?.ToString();
        }

        /// <summary>
        /// IP manzilining to'g'ri formatda ekanligini tekshiradi.
        /// </summary>
        /// <param name="ipAddress">IP manzili</param>
        /// <returns>True - agar IP manzil to'g'ri formatda bo'lsa, aks holda - False</returns>
        private static bool IsValidIpAddress(string ipAddress)
        {
            // IP manzilining to'g'ri formatda ekanligini tekshirish
            return System.Net.IPAddress.TryParse(ipAddress, out _);
        }
    }
}
