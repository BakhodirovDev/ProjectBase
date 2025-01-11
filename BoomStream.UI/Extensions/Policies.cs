namespace BoomStream.UI.Extensions;

public static class Policies
{
    public static void AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("Policies", policy =>
            {
                policy
                    .WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>())
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithExposedHeaders("Content-Disposition");
            });

            options.AddPolicy("CorsPolicyy", builder =>
            {
                builder
                    .WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .SetIsOriginAllowed(_ => true); // Development uchun
            });
        });
    }

    public static void AddAuthorizationPolicy(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("SuperAdmin", policy => policy.RequireRole("SuperAdmin", "Admin"));
            options.AddPolicy("Admin", policy => policy.RequireRole("Role", "Admin"));
            options.AddPolicy("User", policy => policy.RequireRole("Role", "User"));
        });
    }
}
