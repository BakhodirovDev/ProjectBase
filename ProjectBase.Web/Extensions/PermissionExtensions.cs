using Handler;
using Microsoft.AspNetCore.Authorization;
using Provider;

namespace ProjectBase.WebApi.Extensions;

public static class PermissionExtensions
{
    public static IServiceCollection AddPermissionBasedAuthorization(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        return services;
    }
}