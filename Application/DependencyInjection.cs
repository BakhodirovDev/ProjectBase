using Application.Service;
using Domain;
using Domain.Abstraction;
using Domain.Abstraction.Jwt;
using Domain.EfClasses.Authentication;
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
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddHttpClient<IIpGeolocationService, IpGeolocationService>();
        services.AddAutoMapper(cfg => { }, typeof(DependencyInjection).Assembly);

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        services.AddSingleton<ILogger>(logger);

        return services;
    }
}