using Application.Service;
using Application.Service.BaseService;
using Domain;
using Domain.Abstraction;
using Domain.Abstraction.Authentication;
using Domain.Abstraction.Base;
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
        services.AddScoped<JwtTokenService>();
        
        // Authentication Services
        services.AddScoped<IAuthService, AuthService>();
        
        // IP Geolocation Service
        services.AddHttpClient<IIpGeolocationService, IpGeolocationService>();

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        services.AddSingleton<ILogger>(logger);

        return services;
    }
}
