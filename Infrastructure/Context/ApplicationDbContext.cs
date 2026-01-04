using Domain.Abstraction.Base;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Infrastructure.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Apply snake_case naming convention
        ApplySnakeCaseNaming(modelBuilder);

        // Apply global query filters
        ApplyGlobalQueryFilters(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (!optionsBuilder.IsConfigured)
        {
            Console.WriteLine("⚠️ Warning: DbContext is being configured without a connection string.");
        }

        // Enable sensitive data logging in development
#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();
#endif
    }

    private void ApplyGlobalQueryFilters(ModelBuilder modelBuilder)
    {
        // Soft delete filter barcha AuditableEntity'lar uchun
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Check if entity implements IAuditableEntity
            if (typeof(IAuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                // Query filter allaqachon base configuration'da qo'shilgan
                // Bu yerda qo'shimcha global filter'lar qo'shish mumkin
            }
        }
    }

    public IQueryable<T> GetDbSet<T>() where T : class
    {
        return Set<T>();
    }

    private void ApplySnakeCaseNaming(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Table name
            var tableName = entity.GetTableName();
            if (!string.IsNullOrEmpty(tableName))
            {
                entity.SetTableName(ToSnakeCase(tableName));
            }

            // Column names
            foreach (var property in entity.GetProperties())
            {
                var columnName = property.GetColumnName();
                if (!string.IsNullOrEmpty(columnName))
                {
                    property.SetColumnName(ToSnakeCase(columnName));
                }
            }

            // Foreign key constraint names
            foreach (var key in entity.GetForeignKeys())
            {
                var constraintName = key.GetConstraintName();
                if (!string.IsNullOrEmpty(constraintName))
                {
                    key.SetConstraintName(ToSnakeCase(constraintName));
                }
            }

            // Index names
            foreach (var index in entity.GetIndexes())
            {
                var indexName = index.GetDatabaseName();
                if (!string.IsNullOrEmpty(indexName))
                {
                    index.SetDatabaseName(ToSnakeCase(indexName));
                }
            }

            // Primary key constraint name
            var primaryKey = entity.FindPrimaryKey();
            if (primaryKey != null)
            {
                var pkName = primaryKey.GetName();
                if (!string.IsNullOrEmpty(pkName))
                {
                    primaryKey.SetName(ToSnakeCase(pkName));
                }
            }
        }
    }

    private string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Handle acronyms (e.g., "HTTPResponse" -> "http_response")
        var result = Regex.Replace(input, "([A-Z]+)([A-Z][a-z])", "$1_$2");

        // Handle normal camel case (e.g., "createdAt" -> "created_at")
        result = Regex.Replace(result, "([a-z0-9])([A-Z])", "$1_$2");

        // Handle numbers (e.g., "user2Name" -> "user_2_name")
        result = Regex.Replace(result, "([a-zA-Z])([0-9])", "$1_$2");
        result = Regex.Replace(result, "([0-9])([a-zA-Z])", "$1_$2");

        return result.ToLower();
    }
}