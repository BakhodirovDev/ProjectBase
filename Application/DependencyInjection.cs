using Domain.Abstraction.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationRegisterService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<JwtOption>();

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        services.AddSingleton<ILogger>(logger);

        return services;
    }
}
