using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace ProjectBase.WebApi.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAsync(this IHost host, int maxRetries = 5)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();

                logger.LogInformation("🔄 Attempting to connect to database (attempt {Attempt}/{MaxRetries})",
                    attempt, maxRetries);

                // Test database connection
                var canConnect = await context.Database.CanConnectAsync();

                if (!canConnect)
                {
                    throw new InvalidOperationException("Cannot connect to database");
                }

                logger.LogInformation("✅ Database connection successful");

                // Check for pending migrations
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                var pendingMigrationsList = pendingMigrations.ToList();

                if (pendingMigrationsList.Any())
                {
                    logger.LogInformation("📦 Found {Count} pending migration(s):", pendingMigrationsList.Count);

                    foreach (var migration in pendingMigrationsList)
                    {
                        logger.LogInformation("   - {Migration}", migration);
                    }

                    logger.LogInformation("🚀 Applying migrations...");
                    await context.Database.MigrateAsync();

                    logger.LogInformation("✅ All migrations applied successfully");
                }
                else
                {
                    logger.LogInformation("✅ Database is up to date - no pending migrations");
                }

                var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
                logger.LogInformation("📊 Total applied migrations: {Count}", appliedMigrations.Count());

                return;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "⚠️ Migration attempt {Attempt}/{MaxRetries} failed: {Message}",
                    attempt, maxRetries, ex.Message);

                if (attempt == maxRetries)
                {
                    logger.LogError(ex, "❌ Failed to apply migrations after {MaxRetries} attempts", maxRetries);
                    throw new InvalidOperationException(
                        $"Failed to apply database migrations after {maxRetries} attempts. " +
                        "Please check database connection and logs.", ex);
                }

                // Exponential backoff: 2s, 4s, 8s, 16s, 32s
                var delaySeconds = Math.Pow(2, attempt);
                logger.LogInformation("⏳ Retrying in {Delay} seconds...", delaySeconds);
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
            }
        }
    }

    public static async Task EnsureDatabaseCreatedAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        var environment = services.GetRequiredService<IWebHostEnvironment>();

        // Only in development
        if (!environment.IsDevelopment())
        {
            logger.LogInformation("⏭️ Skipping database creation check (not in development mode)");
            return;
        }

        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();

            logger.LogInformation("🔍 Checking if database exists...");

            var exists = await context.Database.CanConnectAsync();

            if (!exists)
            {
                logger.LogInformation("📦 Creating database...");
                await context.Database.EnsureCreatedAsync();
                logger.LogInformation("✅ Database created successfully");
            }
            else
            {
                logger.LogInformation("✅ Database already exists");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Error while ensuring database exists");
            throw;
        }
    }

    public static async Task ValidateDatabaseConnectionAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();

            logger.LogInformation("Validating database connection...");

            var canConnect = await context.Database.CanConnectAsync();

            if (!canConnect)
            {
                throw new InvalidOperationException("Cannot establish database connection");
            }

            var connectionString = context.Database.GetConnectionString();
            var providerName = context.Database.ProviderName;

            logger.LogInformation("Database connection validated");
            logger.LogInformation("Provider: {Provider}", providerName);
            logger.LogInformation("Connection: {Connection}",
                MaskConnectionString(connectionString));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database connection validation failed");
            throw;
        }
    }

    private static string MaskConnectionString(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return "N/A";

        var masked = System.Text.RegularExpressions.Regex.Replace(
            connectionString,
            @"Password=([^;]+)",
            "Password=***",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        return masked;
    }
}