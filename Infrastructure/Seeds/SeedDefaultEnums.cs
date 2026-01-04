using Consts;
using Domain.Models.Enums;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Seeds;

public static class SeedDefaultEnums
{
    public static async Task SeedAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        
        logger.LogInformation("Starting enums seeding...");
        
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var statusesAdded = await SeedStatusAsync(dbContext, logger);
            var gendersAdded = await SeedGenderAsync(dbContext, logger);
            var documentTypesAdded = await SeedDocumentTypeAsync(dbContext, logger);
            var authTypesAdded = await SeedAuthTypeAsync(dbContext, logger);

            await transaction.CommitAsync();
            
            logger.LogInformation(
                "Seeding completed successfully. Statuses: {StatusesCount}, Genders: {GendersCount}, DocumentTypes: {DocumentTypesCount}, AuthTypes: {AuthTypesCount}",
                statusesAdded, gendersAdded, documentTypesAdded, authTypesAdded);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError(ex, "An error occurred while seeding enums. Transaction rolled back.");
            throw;
        }
    }

    /// <summary>
    /// Status konstantalaridan statuslarni database'ga seed qilish
    /// </summary>
    private static async Task<int> SeedStatusAsync(ApplicationDbContext dbContext, ILogger logger)
    {
        var existingStatusCodes = await dbContext.Set<EnumStatus>()
            .AsNoTracking()
            .Select(s => s.Code)
            .ToHashSetAsync();

        var statusesToAdd = new List<EnumStatus>();

        var statusDefinitions = new Dictionary<int, (string Code, string ShortName, string FullName)>
        {
            { StatusIdConst.CREATED, ("CREATED", "Yaratilgan", "Yaratilgan holat") },
            { StatusIdConst.MODIFIED, ("MODIFIED", "O'zgartirilgan", "O'zgartirilgan holat") },
            { StatusIdConst.DELETED, ("DELETED", "O'chirilgan", "O'chirilgan holat") }
        };

        foreach (var (id, (code, shortName, fullName)) in statusDefinitions)
        {
            if (!existingStatusCodes.Contains(code))
            {
                var status = CreateEnumEntity<EnumStatus>(id);
                status.Code = code;
                status.ShortName = shortName;
                status.FullName = fullName;
                statusesToAdd.Add(status);
            }
        }

        foreach (var existingCode in existingStatusCodes)
        {
            if (!statusDefinitions.Any(s => s.Value.Code == existingCode))
            {
                logger.LogWarning("Unknown status code found in database: {Code}", existingCode);
            }
        }

        if (statusesToAdd.Count > 0)
        {
            await dbContext.Set<EnumStatus>().AddRangeAsync(statusesToAdd);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Added {Count} new statuses: {Statuses}",
                statusesToAdd.Count,
                string.Join(", ", statusesToAdd.Select(s => s.Code)));
        }
        else
        {
            logger.LogDebug("No new statuses to add.");
        }

        return statusesToAdd.Count;
    }

    /// <summary>
    /// Gender enum'ini seed qilish
    /// </summary>
    private static async Task<int> SeedGenderAsync(ApplicationDbContext dbContext, ILogger logger)
    {
        var existingCount = await dbContext.Set<EnumGender>().CountAsync();
        
        if (existingCount > 0)
        {
            logger.LogDebug("Genders already seeded. Skipping...");
            return 0;
        }

        var gendersToAdd = new List<EnumGender>();

        var genderDefinitions = new Dictionary<int, (string Code, string ShortName, string FullName)>
        {
            { GenderConst.MALE, ("MALE", "Erkak", "Erkak jins") },
            { GenderConst.FEMALE, ("FEMALE", "Ayol", "Ayol jins") }
        };

        foreach (var (id, (code, shortName, fullName)) in genderDefinitions)
        {
            var gender = CreateEnumEntity<EnumGender>(id);
            gender.Code = code;
            gender.ShortName = shortName;
            gender.FullName = fullName;
            gendersToAdd.Add(gender);
        }

        await dbContext.Set<EnumGender>().AddRangeAsync(gendersToAdd);
        await dbContext.SaveChangesAsync();
        
        logger.LogInformation("Added {Count} genders: {Genders}",
            gendersToAdd.Count,
            string.Join(", ", gendersToAdd.Select(g => g.Code)));

        return gendersToAdd.Count;
    }

    /// <summary>
    /// EnumDocumentType enum'ini seed qilish
    /// </summary>
    private static async Task<int> SeedDocumentTypeAsync(ApplicationDbContext dbContext, ILogger logger)
    {
        var existingDocTypeCodes = await dbContext.Set<EnumDocumentType>()
            .AsNoTracking()
            .Select(d => d.Code)
            .ToHashSetAsync();

        var documentTypesToAdd = new List<EnumDocumentType>();

        var documentTypeDefinitions = new Dictionary<int, (string Code, string ShortName, string FullName)>
        {
            { DocumentTypeConst.CERTIFICATE, ("CERTIFICATE", "Guvohnoma", "Guvohnoma (eski qog'oz hujjat)") },
            { DocumentTypeConst.PASSPORT, ("PASSPORT", "Pasport", "Pasport (biometrik)") },
            { DocumentTypeConst.ID_CARD, ("ID_CARD", "ID-karta", "ID-karta (plastik)") }
        };

        foreach (var (id, (code, shortName, fullName)) in documentTypeDefinitions)
        {
            if (!existingDocTypeCodes.Contains(code))
            {
                var docType = CreateEnumEntity<EnumDocumentType>(id);
                docType.Code = code;
                docType.ShortName = shortName;
                docType.FullName = fullName;
                documentTypesToAdd.Add(docType);
            }
        }

        foreach (var existingCode in existingDocTypeCodes)
        {
            if (!documentTypeDefinitions.Any(d => d.Value.Code == existingCode))
            {
                logger.LogWarning("Unknown document type code found in database: {Code}", existingCode);
            }
        }

        if (documentTypesToAdd.Count > 0)
        {
            await dbContext.Set<EnumDocumentType>().AddRangeAsync(documentTypesToAdd);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Added {Count} new document types: {DocumentTypes}",
                documentTypesToAdd.Count,
                string.Join(", ", documentTypesToAdd.Select(d => d.Code)));
        }
        else
        {
            logger.LogDebug("No new document types to add.");
        }

        return documentTypesToAdd.Count;
    }

    /// <summary>
    /// AuthType enum'ini seed qilish
    /// </summary>
    private static async Task<int> SeedAuthTypeAsync(ApplicationDbContext dbContext, ILogger logger)
    {
        var existingDocTypeCodes = await dbContext.Set<EnumAuthType>()
            .AsNoTracking()
            .Select(d => d.Code)
            .ToHashSetAsync();

        var authTypesToAdd = new List<EnumAuthType>();

        var authTypeDefinitions = new Dictionary<int, (string Code, string ShortName, string FullName)>
        {
            { AuthTypeConst.USERNAME, ("USERNAME", "Foydalanuvchi nomi", "Foydalanuvchi nomi orqali") }
        };

        foreach (var (id, (code, shortName, fullName)) in authTypeDefinitions)
        {
            if (!existingDocTypeCodes.Contains(code))
            {
                var docType = CreateEnumEntity<EnumAuthType>(id);
                docType.Code = code;
                docType.ShortName = shortName;
                docType.FullName = fullName;
                authTypesToAdd.Add(docType);
            }
        }

        foreach (var existingCode in existingDocTypeCodes)
        {
            if (!authTypeDefinitions.Any(d => d.Value.Code == existingCode))
            {
                logger.LogWarning("Unknown auth type code found in database: {Code}", existingCode);
            }
        }

        if (authTypesToAdd.Count > 0)
        {
            await dbContext.Set<EnumAuthType>().AddRangeAsync(authTypesToAdd);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Added {Count} new auth types: {AuthType}",
                authTypesToAdd.Count,
                string.Join(", ", authTypesToAdd.Select(d => d.Code)));
        }
        else
        {
            logger.LogDebug("No new auth types to add.");
        }

        return authTypesToAdd.Count;
    }


    /// <summary>
    /// Generic method: private constructor orqali enum entity yaratish
    /// </summary>
    private static T CreateEnumEntity<T>(int id) where T : class
    {
        return (T)Activator.CreateInstance(
            typeof(T),
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
            null,
            new object[] { id },
            null)!;
    }
}