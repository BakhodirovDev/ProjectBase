using Consts;
using Domain.EfClasses;
using Domain.EfClasses.Authentication;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Seeds;

public static class SeedDefaultPersonAndUser
{
    #region Constants

    /// <summary>
    /// Super Admin role uchun statik ID
    /// </summary>
    private static readonly Guid SuperAdminRoleId = Guid.Parse("b99ca8ea-d24f-4845-977b-7c53ffa4b2e4");

    /// <summary>
    /// Super Admin User uchun statik ID
    /// </summary>
    private static readonly Guid SuperAdminUserId = Guid.Parse("41896b5b-1eb8-401d-a8d6-bb7d0fc5792d");

    /// <summary>
    /// Super Admin person uchun statik ID
    /// </summary>
    private static readonly Guid SuperAdminPersonId = Guid.Parse("84724f6c-e9bb-4d3e-a887-f6710e32337f");

    #endregion

    public static async Task SeedAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = services.GetRequiredService<IPasswordHasher<User>>();

        logger.LogInformation("Starting SuperAdmin seeding...");

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await SeedDefaultSuperAdminAsync(dbContext, passwordHasher, logger);
            await transaction.CommitAsync();
            logger.LogInformation("SuperAdmin seeding completed successfully.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError(ex, "An error occurred while seeding SuperAdmin. Transaction rolled back.");
            throw;
        }
    }

    private static async Task SeedDefaultSuperAdminAsync(
        ApplicationDbContext dbContext,
        IPasswordHasher<User> passwordHasher,
        ILogger logger)
    {
        // Check if SuperAdmin already exists
        var existingSuperAdmin = await dbContext.Set<User>()
            .Where(u => u.Id == SuperAdminUserId && u.StatusId != StatusIdConst.DELETED)
            .FirstOrDefaultAsync();

        if (existingSuperAdmin != null)
        {
            logger.LogInformation("SuperAdmin User already exists. Skipping creation.");
            return;
        }

        // Create Person first
        var person = CreateSuperAdminPerson();
        dbContext.Set<Person>().Add(person);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Created SuperAdmin person: {PersonId}", person.Id);

        // Create User
        var user = CreateSuperAdminUser(person.Id, passwordHasher);
        dbContext.Set<User>().Add(user);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Created SuperAdmin User: {Email} with username: {UserName}",
            user.Email, user.UserName);

        // Assign SuperAdmin role
        await AssignSuperAdminRoleAsync(dbContext, user.Id, logger);
    }

    private static Person CreateSuperAdminPerson()
    {
        // Reflection orqali private constructor chaqirish
        var person = (Person)Activator.CreateInstance(
            typeof(Person),
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
            null,
            Array.Empty<object>(),
            null)!;

        // ID ni set qilish
        person.IdSet(SuperAdminPersonId);

        // Person ma'lumotlarini to'ldirish
        SetPersonProperty(person, nameof(Person.FirstName), "Super");
        SetPersonProperty(person, nameof(Person.MiddleName), "Admin");
        SetPersonProperty(person, nameof(Person.LastName), "User");
        SetPersonProperty(person, nameof(Person.ShortName), "S.A.U");
        SetPersonProperty(person, nameof(Person.FullName), "Super Admin User");
        SetPersonProperty(person, nameof(Person.GenderId), GenderConst.MALE);

        SetPersonProperty(person, nameof(Person.CountryId), 1); // Uzbekistan
        SetPersonProperty(person, nameof(Person.RegionId), 1); // Toshkent shahar
        SetPersonProperty(person, nameof(Person.DistrictId), 1); // Default district
        SetPersonProperty(person, nameof(Person.NationalityId), 1); // O'zbek
        SetPersonProperty(person, nameof(Person.Address), "Tashkent, Uzbekistan");

        return person;
    }

    private static User CreateSuperAdminUser(Guid personId, IPasswordHasher<User> passwordHasher)
    {
        // Reflection orqali private constructor chaqirish
        var user = (User)Activator.CreateInstance(
            typeof(User),
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
            null,
            new object[] { SuperAdminUserId },
            null)!;

        // ID ni set qilish
        user.IdSet(SuperAdminUserId);

        // User ma'lumotlarini to'ldirish
        SetUserProperty(user, nameof(User.PersonId), personId);
        SetUserProperty(user, nameof(User.UserName), "superadmin");
        SetUserProperty(user, nameof(User.Email), "superadmin@projectbase");
        SetUserProperty(user, nameof(User.IsEmailConfirmed), true);

        // Password hash qilish
        var defaultPassword = "Admin123!";
        var hashedPassword = passwordHasher.HashPassword(user, defaultPassword);
        SetUserProperty(user, nameof(User.PasswordHash), hashedPassword);

        return user;
    }

    private static async Task AssignSuperAdminRoleAsync(
        ApplicationDbContext dbContext,
        Guid userId,
        ILogger logger)
    {
        // Get SuperAdmin role
        var superAdminRole = await dbContext.Set<Role>()
            .Where(r => r.Id == SuperAdminRoleId && r.StatusId != StatusIdConst.DELETED)
            .FirstOrDefaultAsync();

        if (superAdminRole == null)
        {
            logger.LogWarning(
                "SuperAdmin role with ID {RoleId} not found. Make sure to run SeedPermissionsAndRoles first.",
                SuperAdminRoleId);
            return;
        }

        // Check if User already has SuperAdmin role
        var existingUserRole = await dbContext.Set<UserRole>()
            .Where(ur => ur.UserId == userId && ur.RoleId == superAdminRole.Id)
            .FirstOrDefaultAsync();

        if (existingUserRole != null)
        {
            logger.LogInformation("User already has SuperAdmin role assigned.");
            return;
        }

        // Assign SuperAdmin role to User
        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = superAdminRole.Id
        };

        dbContext.Set<UserRole>().Add(userRole);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Assigned SuperAdmin role to User: {UserId}", userId);
    }

    #region Helper Methods

    private static void SetPersonProperty(Person person, string propertyName, object value)
    {
        var property = typeof(Person).GetProperty(propertyName);
        if (property != null && property.CanWrite)
        {
            property.SetValue(person, value);
        }
    }

    private static void SetUserProperty(User user, string propertyName, object value)
    {
        var property = typeof(User).GetProperty(propertyName);
        if (property != null && property.CanWrite)
        {
            property.SetValue(user, value);
        }
    }

    #endregion
}