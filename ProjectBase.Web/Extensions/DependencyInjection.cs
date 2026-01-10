using Application;
using Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using StackExchange.Redis;
using System.Threading.RateLimiting;

namespace ProjectBase.WebApi.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration,
                                                            IWebHostEnvironment environment)
    {
        services.AddLogging();
        services.AddControllers();
        services.AddApplicationRegisterService(configuration);
        services.AddInfrastructureRegisterService(configuration);

        // Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddSwaggerGenWithBearer(environment, configuration);

        // JWT Authentication
        services.AddJwt(configuration);

        // CORS
        services.AddCorsPolicy(configuration);

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

        // Redis configuration
        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            var redisOptions = ConfigurationOptions.Parse(redisConnectionString);
            redisOptions.AbortOnConnectFail = false;
            redisOptions.ConnectTimeout = 5000;
            redisOptions.SyncTimeout = 5000;
            redisOptions.ConnectRetry = 3;
            redisOptions.KeepAlive = 60;
            redisOptions.DefaultDatabase = 0;

            // SignalR with Redis
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
                        Console.WriteLine($"Redis connection failed: {e.Exception?.Message}");
                    };

                    connection.ConnectionRestored += (_, e) =>
                    {
                        Console.WriteLine("Redis connection restored.");
                    };

                    return connection;
                };
            });
        }

        // Rate Limiter
        var rateLimiterOptions = new Domain.Abstraction.Options.RateLimitOptions.RateLimiterOptions();
        configuration.GetSection("RateLimiter").Bind(rateLimiterOptions);

        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var userKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(userKey, _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = rateLimiterOptions.GlobalLimiter.PermitLimit,
                        Window = TimeSpan.FromMinutes(rateLimiterOptions.GlobalLimiter.WindowInMinutes),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = rateLimiterOptions.GlobalLimiter.QueueLimit
                    });
            });

            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    await context.HttpContext.Response.WriteAsync(
                        $"Too many requests. Please retry after {retryAfter.TotalSeconds} seconds.",
                        token);
                }
                else
                {
                    await context.HttpContext.Response.WriteAsync(
                        "Too many requests. Please try again later.",
                        token);
                }
            };
        });

        return services;
    }
}