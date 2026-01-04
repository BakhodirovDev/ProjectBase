using Domain.Abstraction.Base;
using Domain.EfClasses;
using Domain.EfClasses.Authentication;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureRegisterService(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                                 throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddDatabaseHealthChecks(configuration);
        services.AddScoped(typeof(IBaseRepository<>), typeof(Repository<>));
        services.AddScoped(typeof(IBaseRepository<,>), typeof(Repository<,>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        
        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        
        return services;
    }

    #region Health Checks

    private static IServiceCollection AddDatabaseHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;

        services.AddHealthChecks()
            .AddNpgSql(
                connectionString,
                name: "postgresql",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "db", "postgresql", "ready" },
                timeout: TimeSpan.FromSeconds(5))
            .AddDbContextCheck<ApplicationDbContext>(
                name: "dbcontext",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "db", "efcore", "ready" });

        return services;
    }

    #endregion
}