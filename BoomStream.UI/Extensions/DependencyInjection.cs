using Application;
using Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using StackExchange.Redis;
using System.Threading.RateLimiting;

namespace BoomStream.UI.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration,
                                                            IWebHostEnvironment environment)
    {
        services.AddControllers();
        services.AddApplicationRegisterService();
        services.AddInfrastructureRegisterService(configuration);
        services.AddOpenApi();
        services.AddSwaggerGen();
        services.AddSwaggerGenWithBearer();
        services.AddJwt(configuration);

        // Cors sozlash
        services.AddCorsPolicy(configuration);

        // Authorization Role sozlash
        services.AddAuthorizationPolicy();

        // Response Compression sozlash
        services.AddResponseCompression(options =>
        {
            // HTTPS so'rovlarida siqishni yoqish
            options.EnableForHttps = true;

            // Siqishni qo'llash uchun MIME turlari ro'yxati
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
            {
                "application/json",       // JSON
                "text/html",              // HTML
                "text/plain",             // Oddiy matn
                "application/xml",        // XML
                "application/javascript", // JavaScript
                "text/css",               // CSS
                "image/svg+xml",          // SVG rasm
                "application/font-woff",  // WOFF font
                "application/font-woff2", // WOFF2 font
                "image/png",              // PNG rasm
                "image/jpeg",             // JPEG rasm
                "image/gif",              // GIF rasm
                "video/mp4",              // MP4 video
                "audio/mpeg"              // MP3 audio
            });

            // Siqish provayderlarini qo'shish (Gzip va Brotli)
            options.Providers.Add<GzipCompressionProvider>();
            options.Providers.Add<BrotliCompressionProvider>();
        });

        // Serilogni sozlash
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        // Redis configuration options
        var redisOptions = new ConfigurationOptions
        {
            EndPoints = { configuration.GetConnectionString("Redis").Split(',')[0] },
            AbortOnConnectFail = false,
            ConnectTimeout = 5000,
            SyncTimeout = 5000,
            ConnectRetry = 3,
            KeepAlive = 60,
            DefaultDatabase = 0
        };
        // SignalR configuration
        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = environment.IsDevelopment();
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
            options.MaximumReceiveMessageSize = 32 * 1024;
        })
        .AddStackExchangeRedis(options =>
        {
            options.Configuration = redisOptions;
            options.ConnectionFactory = async writer =>
            {
                var connection = await ConnectionMultiplexer.ConnectAsync(redisOptions, writer);
                connection.ConnectionFailed += (_, e) =>
                {
                    Console.WriteLine("Connection to Redis failed.");
                };
                connection.ConnectionRestored += (_, e) =>
                {
                    Console.WriteLine("Connection to Redis restored.");
                };
                return connection;
            };
        });

        // RateLimiter sozlamalarini yuklash
        var rateLimiterOptions = new Domain.Abstraction.Options.RateLimitOptions.RateLimiterOptions();
        configuration.GetSection("RateLimiter").Bind(rateLimiterOptions);

        // RateLimiter sozlamalarini qo'shish
        services.AddRateLimiter(options =>
        {   
            // Foydalanuvchini identifikatsiya qilish uchun IP yoki boshqa kalitni ajratib olish
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                // Foydalanuvchini aniqlash uchun IP yoki User ID ishlatiladi
                var userKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                // Har bir foydalanuvchi uchun Fixed Window limiter yaratish
                return RateLimitPartition.GetFixedWindowLimiter(userKey, key =>
                    new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = rateLimiterOptions.GlobalLimiter.PermitLimit, // Har bir 1 soniya ichida maksimal n ta so'rovga ruxsat
                        Window = TimeSpan.FromSeconds(rateLimiterOptions.GlobalLimiter.WindowInMinutes * 60), // x soniya vaqt oralig'i
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst, // Navbatga qo'shilgan eski so'rov birinchi qayta ishlanadi
                        QueueLimit = rateLimiterOptions.GlobalLimiter.QueueLimit // n ta qo'shimcha so'rov uchun navbatga ruxsat beriladi
                    });
            });
        });


        return services;
    }
}
