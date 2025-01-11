using Domain.Abstraction.Jwt;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationRegisterService(this IServiceCollection services)
    {

        services.AddScoped<JwtOption>();
        services.AddHttpContextAccessor();
        return services;
    }
}
