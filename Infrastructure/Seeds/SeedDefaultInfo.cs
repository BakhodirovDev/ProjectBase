using Consts;
using Domain.EfClasses.Info;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Text;

namespace Infrastructure.Seeds;

public static class SeedDefaultInfo
{
    public static async Task SeedAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
        var dbContext = services.GetRequiredService<ApplicationDbContext>();

        logger.LogInformation("Starting database seeding process...");

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var nationalitiesAdded = await SeedNationalitiesAsync(dbContext, logger);
            var countriesAdded = await SeedCountriesAsync(dbContext, logger);
            var regionsAdded = await SeedRegionsAsync(dbContext, logger);
            var districtsAdded = await SeedDistrictsAsync(dbContext, logger);

            await transaction.CommitAsync();

            logger.LogInformation(
                "Seeding completed successfully. Countries: {CountriesCount}, Regions: {RegionsCount}, Districts: {DistrictsCount}",
                countriesAdded, regionsAdded, districtsAdded);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError(ex, "An error occurred during seeding. Transaction rolled back.");
            throw;
        }
    }

    private static async Task<int> SeedNationalitiesAsync(ApplicationDbContext dbContext, ILogger logger)
    {
        var nationalityDefinitions = new Dictionary<int, (string Code, string ShortName, string FullName)>
        {
            { NationalityIdConst.Uzbekistan, ("UZ", "O'zbek", "O'zbekiston Respublikasi fuqaroligi") },
            { NationalityIdConst.Kyrgyzstan, ("KG", "Qirg'iz", "Qirg'iz Respublikasi fuqaroligi") },
            { NationalityIdConst.Tajikistan, ("TJ", "Tojik", "Tojikiston Respublikasi fuqaroligi") },
            { NationalityIdConst.Turkmenistan, ("TM", "Turkman", "Turkmaniston Respublikasi fuqaroligi") },
            { NationalityIdConst.Kazakhstan, ("KZ", "Qozog'", "Qozog'iston Respublikasi fuqaroligi") },
            { NationalityIdConst.Russia, ("RU", "Rus", "Rossiya Federatsiyasi fuqaroligi") }
        };

        return await SeedInfoEntitiesAsync(
            dbContext,
            logger,
            nationalityDefinitions,
            (id, data) =>
            {
                var nationality = CreateInfoEntity<InfoNationality>(id);
                nationality.Code = data.Code;
                nationality.ShortName = data.ShortName;
                nationality.FullName = data.FullName;
                return nationality;
            },
            n => n.Code,
            "Nationalities"
        );
    }

    private static async Task<int> SeedCountriesAsync(ApplicationDbContext dbContext, ILogger logger)
    {
        var countryDefinitions = new Dictionary<int, (string Code, string ShortName, string FullName)>
        {
            { CountryIdConst.UZBEKISTAN, ("UZ", "O'zbekiston", "O'zbekiston Respublikasi") }
        };

        return await SeedInfoEntitiesAsync(
            dbContext,
            logger,
            countryDefinitions,
            (id, data) =>
            {
                var country = CreateInfoEntity<InfoCountry>(id);
                country.Code = data.Code;
                country.ShortName = data.ShortName;
                country.FullName = data.FullName;
                return country;
            },
            c => c.Code,
            "Countries"
        );
    }

    private static async Task<int> SeedRegionsAsync(ApplicationDbContext dbContext, ILogger logger)
    {
        var regionDefinitions = new Dictionary<int, (int CountryId, string Code, string Soato, string RoamingCode, string ShortName, string FullName)>
        {
            { RegionIdConst.TASHKENT_CITY, (CountryIdConst.UZBEKISTAN, "TAS", "1726", "26", "Toshkent sh.", "Toshkent shahri") },
            { RegionIdConst.TASHKENT_REGION, (CountryIdConst.UZBEKISTAN, "TASR", "1727", "27", "Toshkent vil.", "Toshkent viloyati") },
            { RegionIdConst.ANDIJAN_REGION, (CountryIdConst.UZBEKISTAN, "AND", "1703", "3", "Andijon vil.", "Andijon viloyati") },
            { RegionIdConst.BUKHARA_REGION, (CountryIdConst.UZBEKISTAN, "BUK", "1706", "6", "Buxoro vil.", "Buxoro viloyati") },
            { RegionIdConst.JIZZAKH_REGION, (CountryIdConst.UZBEKISTAN, "JIZ", "1708", "8", "Jizzax vil.", "Jizzax viloyati") },
            { RegionIdConst.KASHKADARYA_REGION, (CountryIdConst.UZBEKISTAN, "KAS", "1710", "10", "Qashqadaryo vil.", "Qashqadaryo viloyati") },
            { RegionIdConst.NAVOI_REGION, (CountryIdConst.UZBEKISTAN, "NAV", "1712", "12", "Navoiy vil.", "Navoiy viloyati") },
            { RegionIdConst.NAMANGAN_REGION, (CountryIdConst.UZBEKISTAN, "NAM", "1714", "14", "Namangan vil.", "Namangan viloyati") },
            { RegionIdConst.SAMARKAND_REGION, (CountryIdConst.UZBEKISTAN, "SAM", "1718", "18", "Samarqand vil.", "Samarqand viloyati") },
            { RegionIdConst.SURKHANDARYA_REGION, (CountryIdConst.UZBEKISTAN, "SUR", "1722", "22", "Surxondaryo vil.", "Surxondaryo viloyati") },
            { RegionIdConst.SYRDARYA_REGION, (CountryIdConst.UZBEKISTAN, "SYR", "1724", "24", "Sirdaryo vil.", "Sirdaryo viloyati") },
            { RegionIdConst.FERGHANA_REGION, (CountryIdConst.UZBEKISTAN, "FER", "1730", "30", "Farg'ona vil.", "Farg'ona viloyati") },
            { RegionIdConst.KHOREZM_REGION, (CountryIdConst.UZBEKISTAN, "XOR", "1733", "33", "Xorazm vil.", "Xorazm viloyati") },
            { RegionIdConst.REPUBLIC_OF_KARAKALPAKSTAN, (CountryIdConst.UZBEKISTAN, "QAR", "1735", "35", "Qoraqalpog'iston Respublikasi", "Qoraqalpog'iston Respublikasi") }
        };

        return await SeedInfoEntitiesAsync(
            dbContext,
            logger,
            regionDefinitions,
            (id, data) =>
            {
                var region = CreateInfoEntity<InfoRegion>(id);
                region.CountryId = data.CountryId;
                region.Code = data.Code;
                region.Soato = data.Soato;
                region.RoamingCode = data.RoamingCode;
                region.ShortName = data.ShortName;
                region.FullName = data.FullName;
                return region;
            },
            r => r.Code,
            "Regions"
        );
    }

    private static async Task<int> SeedDistrictsAsync(ApplicationDbContext dbContext, ILogger logger)
    {
        var districtDefinitions = new Dictionary<int, (int RegionId, string? Soato, string? RoamingCode, string Name, bool IsCenter)>
        {
            // Toshkent shahri (12 ta tuman) - Barchasi markaz
            { DistrictIdConst.BEKTEMIR, (RegionIdConst.TASHKENT_CITY, "1726264", "2611", "Bektemir", true) },
            { DistrictIdConst.MIRZO_ULUGBEK, (RegionIdConst.TASHKENT_CITY, "1726269", "2602", "Mirzo Ulug'bek", true) },
            { DistrictIdConst.MIROBOD, (RegionIdConst.TASHKENT_CITY, "1726273", "2601", "Mirobod", true) },
            { DistrictIdConst.OLMAZOR, (RegionIdConst.TASHKENT_CITY, "1726280", "2609", "Olmazor", true) },
            { DistrictIdConst.SERGELI, (RegionIdConst.TASHKENT_CITY, "1726283", "2607", "Sergeli", true) },
            { DistrictIdConst.UCHTEPA, (RegionIdConst.TASHKENT_CITY, "1726262", "2610", "Uchtepa", true) },
            { DistrictIdConst.YASHNOBOD, (RegionIdConst.TASHKENT_CITY, "1726290", "2608", "Yashnobod", true) },
            { DistrictIdConst.CHILONZOR, (RegionIdConst.TASHKENT_CITY, "1726294", "2606", "Chilonzor", true) },
            { DistrictIdConst.SHAYXONTOHUR, (RegionIdConst.TASHKENT_CITY, "1726277", "2605", "Shayxontohur", true) },
            { DistrictIdConst.YUNUSOBOD, (RegionIdConst.TASHKENT_CITY, "1726266", "2603", "Yunusobod", true) },
            { DistrictIdConst.YAKKASAROY, (RegionIdConst.TASHKENT_CITY, "1726287", "2604", "Yakkasaroy", true) },
            { DistrictIdConst.YANGIHAYOT, (RegionIdConst.TASHKENT_CITY, "1726292", null, "Yangihayot", true) },

            // Toshkent viloyati (22 ta)
            { DistrictIdConst.BOSTANLIQ, (RegionIdConst.TASHKENT_REGION, "1727224", "2703", "Bo'stonliq", false) },
            { DistrictIdConst.OQQORGON, (RegionIdConst.TASHKENT_REGION, "1727206", "2707", "Oqqo'rg'on", false) },
            { DistrictIdConst.OXANGARON_T, (RegionIdConst.TASHKENT_REGION, "1727212", "2708", "Oxangaron", false) },
            { DistrictIdConst.BEKOBOD_T, (RegionIdConst.TASHKENT_REGION, "1727220", "2701", "Bekobod", false) },
            { DistrictIdConst.BUKA, (RegionIdConst.TASHKENT_REGION, "1727228", "2702", "Buka", false) },
            { DistrictIdConst.ZANGIOTA, (RegionIdConst.TASHKENT_REGION, "1727237", "2704", "Zangiota", false) },
            { DistrictIdConst.QIBRAY, (RegionIdConst.TASHKENT_REGION, "1727248", "2706", "Qibray", false) },
            { DistrictIdConst.QUYICHIRCHIQ, (RegionIdConst.TASHKENT_REGION, "1727233", "2714", "Quyichirchiq", false) },
            { DistrictIdConst.PARKENT, (RegionIdConst.TASHKENT_REGION, "1727249", "2709", "Parkent", false) },
            { DistrictIdConst.PSKENT, (RegionIdConst.TASHKENT_REGION, "1727250", "2710", "Pskent", false) },
            { DistrictIdConst.TOSHKENT_T, (RegionIdConst.TASHKENT_REGION, "1727265", "2711", "Toshkent", false) },
            { DistrictIdConst.ORTACHIRCHIQ, (RegionIdConst.TASHKENT_REGION, "1727253", "2712", "O'rtachirchiq", false) },
            { DistrictIdConst.CHINOZ, (RegionIdConst.TASHKENT_REGION, "1727256", "2713", "Chinoz", false) },
            { DistrictIdConst.YUQORICHIRCHIQ, (RegionIdConst.TASHKENT_REGION, "1727239", "2705", "Yuqorichirchiq", false) },
            { DistrictIdConst.YANGIYOL_T, (RegionIdConst.TASHKENT_REGION, "1727259", "2715", "Yangiyo'l", false) },
            { DistrictIdConst.OLMALIQ_CITY, (RegionIdConst.TASHKENT_REGION, "1727404", "2718", "Olmaliq shahri", false) },
            { DistrictIdConst.ANGREN_CITY, (RegionIdConst.TASHKENT_REGION, "1727407", "2716", "Angren shahri", false) },
            { DistrictIdConst.OXANGARON_CITY, (RegionIdConst.TASHKENT_REGION, "1727415", "2719", "Oxangaron shahri", false) },
            { DistrictIdConst.BEKOBOD_CITY, (RegionIdConst.TASHKENT_REGION, "1727413", "2717", "Bekobod shahri", false) },
            { DistrictIdConst.CHIRCHIQ_CITY, (RegionIdConst.TASHKENT_REGION, "1727419", "2720", "Chirchiq shahri", false) },
            { DistrictIdConst.YANGIYOL_CITY, (RegionIdConst.TASHKENT_REGION, "1727424", "2722", "Yangiyo'l shahri", false) },
            { DistrictIdConst.NURAFSHON_CITY, (RegionIdConst.TASHKENT_REGION, "1727401", null, "Nurafshon shahri", true) },

            // Andijon viloyati (18 ta)
            { DistrictIdConst.ANDIJON_CITY, (RegionIdConst.ANDIJAN_REGION, "1703401", "0301", "Andijon shahri", true) },
            { DistrictIdConst.ASAKA_CITY, (RegionIdConst.ANDIJAN_REGION, "1703224501", null, "Asaka shahri", false) },
            { DistrictIdConst.OLTINKUL, (RegionIdConst.ANDIJAN_REGION, "1703202", "0306", "Oltinkul", false) },
            { DistrictIdConst.ANDIJON_D, (RegionIdConst.ANDIJAN_REGION, "1703203", "0307", "Andijon", false) },
            { DistrictIdConst.ASAKA, (RegionIdConst.ANDIJAN_REGION, "1703224", "0315", "Asaka", false) },
            { DistrictIdConst.BALIQCHI, (RegionIdConst.ANDIJAN_REGION, "1703206", "0308", "Baliqchi", false) },
            { DistrictIdConst.BUSTON, (RegionIdConst.ANDIJAN_REGION, "1703209", "0309", "Buston", false) },
            { DistrictIdConst.BULOQBOSHI, (RegionIdConst.ANDIJAN_REGION, "1703210", "0310", "Buloqboshi", false) },
            { DistrictIdConst.JALAQUDUQ, (RegionIdConst.ANDIJAN_REGION, "1703211", "0311", "Jalaquduq", false) },
            { DistrictIdConst.IZBOSKAN, (RegionIdConst.ANDIJAN_REGION, "1703214", "0312", "Izboskan", false) },
            { DistrictIdConst.QORGONTEPA, (RegionIdConst.ANDIJAN_REGION, "1703220", "0314", "Qo'rg'ontepa", false) },
            { DistrictIdConst.MARXAMAT, (RegionIdConst.ANDIJAN_REGION, "1703227", "0316", "Marxamat", false) },
            { DistrictIdConst.PAXTAOBOD, (RegionIdConst.ANDIJAN_REGION, "1703232", "0318", "Paxtaobod", false) },
            { DistrictIdConst.ULUGNOR, (RegionIdConst.ANDIJAN_REGION, "1703217", "0313", "Ulug'nor", false) },
            { DistrictIdConst.XOJAOBOD, (RegionIdConst.ANDIJAN_REGION, "1703236", "0319", "Xo'jaobod", false) },
            { DistrictIdConst.XONOBOD_CITY, (RegionIdConst.ANDIJAN_REGION, "1703408", "0303", "Xonobod shahri", false) },
            { DistrictIdConst.SHAHRIXON, (RegionIdConst.ANDIJAN_REGION, "1703230", "0317", "Shahrixon", false) },
            { DistrictIdConst.QORASU_CITY, (RegionIdConst.ANDIJAN_REGION, "1718224554", null, "Qorasu shahri", false) },

            // Buxoro viloyati (13 ta)
            { DistrictIdConst.BUXORO_CITY, (RegionIdConst.BUKHARA_REGION, "1706401", "0601", "Buxoro shahri", true) },
            { DistrictIdConst.KOGON_CITY, (RegionIdConst.BUKHARA_REGION, "1706403", "0603", "Kogon shahri", false) },
            { DistrictIdConst.OLOT, (RegionIdConst.BUKHARA_REGION, "1706204", "0604", "Olot", false) },
            { DistrictIdConst.BUXORO_D, (RegionIdConst.BUKHARA_REGION, "1706207", "0605", "Buxoro", false) },
            { DistrictIdConst.VOBKENT, (RegionIdConst.BUKHARA_REGION, "1706212", "0606", "Vobkent", false) },
            { DistrictIdConst.GIJDUVON, (RegionIdConst.BUKHARA_REGION, "1706215", "0607", "G'ijduvon", false) },
            { DistrictIdConst.JONDOR, (RegionIdConst.BUKHARA_REGION, "1706246", "0608", "Jondor", false) },
            { DistrictIdConst.KOGON_D, (RegionIdConst.BUKHARA_REGION, "1706219", "0609", "Kogon", false) },
            { DistrictIdConst.QORAKOL, (RegionIdConst.BUKHARA_REGION, "1706230", "0610", "Qorako'l", false) },
            { DistrictIdConst.PESHKU, (RegionIdConst.BUKHARA_REGION, "1706240", "0611", "Peshku", false) },
            { DistrictIdConst.ROMITAN, (RegionIdConst.BUKHARA_REGION, "1706242", "0612", "Romitan", false) },
            { DistrictIdConst.QOROVULBOZOR, (RegionIdConst.BUKHARA_REGION, "1706232", "0614", "Qorovulbozor", false) },
            { DistrictIdConst.SHOFIRKON, (RegionIdConst.BUKHARA_REGION, "1706258", "0613", "Shofirkon", false) },

            // Jizzax viloyati (13 ta)
            { DistrictIdConst.ARNASOY, (RegionIdConst.JIZZAKH_REGION, "1708201", "0801", "Arnasoy", false) },
            { DistrictIdConst.YANGIOBOD, (RegionIdConst.JIZZAKH_REGION, "1708237", "0813", "Yangiobod", false) },
            { DistrictIdConst.BAXMAL, (RegionIdConst.JIZZAKH_REGION, "1708204", "0802", "Baxmal", false) },
            { DistrictIdConst.GALLAOROL, (RegionIdConst.JIZZAKH_REGION, "1708209", "0803", "G'allaorol", false) },
            { DistrictIdConst.DUSTLIK, (RegionIdConst.JIZZAKH_REGION, "1708215", "0805", "Dustlik", false) },
            { DistrictIdConst.PAXTAKOR, (RegionIdConst.JIZZAKH_REGION, "1708228", "0810", "Paxtakor", false) },
            { DistrictIdConst.ZOMIN, (RegionIdConst.JIZZAKH_REGION, "1708218", "0807", "Zomin", false) },
            { DistrictIdConst.SHAROF_RASHIDOV, (RegionIdConst.JIZZAKH_REGION, "1708212", "0804", "Sharof Rashidov", false) },
            { DistrictIdConst.ZARBDOR, (RegionIdConst.JIZZAKH_REGION, "1708220", "0806", "Zarbdor", false) },
            { DistrictIdConst.ZAFAROBOD, (RegionIdConst.JIZZAKH_REGION, "1708225", "0808", "Zafarobod", false) },
            { DistrictIdConst.MIRZACHOL, (RegionIdConst.JIZZAKH_REGION, "1708223", "0809", "Mirzacho'l", false) },
            { DistrictIdConst.FORISH, (RegionIdConst.JIZZAKH_REGION, "1708235", "0811", "Forish", false) },
            { DistrictIdConst.JIZZAX_CITY, (RegionIdConst.JIZZAKH_REGION, "1708401", "0812", "Jizzax shahri", true) },

            // Qoraqalpog'iston (17 ta)
            { DistrictIdConst.AMUDARYO, (RegionIdConst.REPUBLIC_OF_KARAKALPAKSTAN, "1735204", "3508", "Amudaryo", false) },
            { DistrictIdConst.BERUNIY, (RegionIdConst.REPUBLIC_OF_KARAKALPAKSTAN, "1735207", "3509", "Beruniy", false) },
            { DistrictIdConst.QONLIKOL, (RegionIdConst.REPUBLIC_OF_KARAKALPAKSTAN, "1735218", "3513", "Qonliko'l", false) },
            { DistrictIdConst.QORAOZAK, (RegionIdConst.REPUBLIC_OF_KARAKALPAKSTAN, "1735211", "3522", "Qorao'zak", false) },
            { DistrictIdConst.KEGEYLI, (RegionIdConst.REPUBLIC_OF_KARAKALPAKSTAN, "1735212", "3511", "Kegeyli", false) },
            { DistrictIdConst.QONGIROT, (RegionIdConst.REPUBLIC_OF_KARAKALPAKSTAN, "1735215", "3512", "Qo'ng'irot", false) },
            { DistrictIdConst.MOYNOQ, (RegionIdConst.REPUBLIC_OF_KARAKALPAKSTAN, "1735222", "3514", "Mo'ynoq", false) },
            { DistrictIdConst.NUKUS_D, (RegionIdConst.REPUBLIC_OF_KARAKALPAKSTAN, "1735225", "3515", "Nukus", false) },
            { DistrictIdConst.TAXTAKOPIR, (RegionIdConst.REPUBLIC_OF_KARAKALPAKSTAN, "1735230", "3516", "Taxtako'pir", false) },
            { DistrictIdConst.TORTKOL, (RegionIdConst.REPUBLIC_OF_KARAKALPAKSTAN, "1735233", "3517", "To'rtko'l", false) },
            { DistrictIdConst.XOJAYLI, (RegionIdConst.REPUBLIC_OF_KARAKALPAKSTAN, "1735236", "3518", "Xo'jayli", false) },
            { DistrictIdConst.CHIMBOY, (RegionIdConst.REPUBLIC_OF_KARAKALPAKSTAN, "1735240", "3519", "Chimboy", false) },
            { DistrictIdConst.SHUMANAY, (RegionIdConst.REPUBLIC_OF_KARAKALPAKSTAN, "1735243", "3520", "Shumanay", false) },
            { DistrictIdConst.ELLIKQALA, (RegionIdConst.REPUBLIC_OF_KARAKALPAKSTAN, "1735250", "3521", "Ellikqal'a", false) },
            { DistrictIdConst.NUKUS_CITY, (RegionIdConst.REPUBLIC_OF_KARAKALPAKSTAN, "1735401", "3501", "Nukus shahri", true) },
            { DistrictIdConst.TAXIATOSH_CITY, (RegionIdConst.REPUBLIC_OF_KARAKALPAKSTAN, "1735228501", "3523", "Taxiatosh shahri", false) },
            { DistrictIdConst.BUZATOV, (RegionIdConst.REPUBLIC_OF_KARAKALPAKSTAN, "1735209", null, "Buzatov", false) },

            // Qashqadaryo viloyati (16 ta)
            { DistrictIdConst.GUZOR, (RegionIdConst.KASHKADARYA_REGION, "1710207", "1004", "G'uzor", false) },
            { DistrictIdConst.DEHQONOBOD, (RegionIdConst.KASHKADARYA_REGION, "1710212", "1005", "Dehqonobod", false) },
            { DistrictIdConst.QAMASHI, (RegionIdConst.KASHKADARYA_REGION, "1710220", "1006", "Qamashi", false) },
            { DistrictIdConst.QARSHI_D, (RegionIdConst.KASHKADARYA_REGION, "1710224", "1007", "Qarshi", false) },
            { DistrictIdConst.KOSON, (RegionIdConst.KASHKADARYA_REGION, "1710229", "1008", "Koson", false) },
            { DistrictIdConst.KASBI, (RegionIdConst.KASHKADARYA_REGION, "1710237", "1012", "Kasbi", false) },
            { DistrictIdConst.KITOB, (RegionIdConst.KASHKADARYA_REGION, "1710232", "1009", "Kitob", false) },
            { DistrictIdConst.MIRISHKOR, (RegionIdConst.KASHKADARYA_REGION, "1710233", "1017", "Mirishkor", false) },
            { DistrictIdConst.MUBORAK, (RegionIdConst.KASHKADARYA_REGION, "1710234", "1010", "Muborak", false) },
            { DistrictIdConst.NISHON, (RegionIdConst.KASHKADARYA_REGION, "1710235", "1011", "Nishon", false) },
            { DistrictIdConst.CHIROQCHI, (RegionIdConst.KASHKADARYA_REGION, "1710242", "1014", "Chiroqchi", false) },
            { DistrictIdConst.SHAHRISABZ_D, (RegionIdConst.KASHKADARYA_REGION, "1710245", "1015", "Shahrisabz", false) },
            { DistrictIdConst.YAKKABOG, (RegionIdConst.KASHKADARYA_REGION, "1710250", "1016", "Yakkabog'", false) },
            { DistrictIdConst.QARSHI_CITY, (RegionIdConst.KASHKADARYA_REGION, "1710401", "1001", "Qarshi shahri", true) },
            { DistrictIdConst.SHAHRISABZ_CITY, (RegionIdConst.KASHKADARYA_REGION, "1710405", null, "Shahrisabz shahri", false) },
            { DistrictIdConst.KOKDALA, (RegionIdConst.KASHKADARYA_REGION, "1710240", null, "Ko'kdala", false) },

            // Navoiy viloyati (11 ta)
            { DistrictIdConst.KONIMEX, (RegionIdConst.NAVOI_REGION, "1712211", "1204", "Konimex", false) },
            { DistrictIdConst.QIZILTEPA, (RegionIdConst.NAVOI_REGION, "1712216", "1205", "Qiziltepa", false) },
            { DistrictIdConst.NAVBAHOR, (RegionIdConst.NAVOI_REGION, "1712230", "1208", "Navbahor", false) },
            { DistrictIdConst.KARMANA, (RegionIdConst.NAVOI_REGION, "1712234", "1211", "Karmana", false) },
            { DistrictIdConst.NUROTA, (RegionIdConst.NAVOI_REGION, "1712238", "1210", "Nurota", false) },
            { DistrictIdConst.TOMDI, (RegionIdConst.NAVOI_REGION, "1712244", "1207", "Tomdi", false) },
            { DistrictIdConst.UCHQUDUQ, (RegionIdConst.NAVOI_REGION, "1712248", "1203", "Uchquduq", false) },
            { DistrictIdConst.XATIRCHI, (RegionIdConst.NAVOI_REGION, "1712251", "1209", "Xatirchi", false) },
            { DistrictIdConst.ZARAFSHON_CITY, (RegionIdConst.NAVOI_REGION, "1712408", "1202", "Zarafshon", false) },
            { DistrictIdConst.NAVOIY_CITY, (RegionIdConst.NAVOI_REGION, "1712401", "1201", "Navoiy shahri", true) },
            { DistrictIdConst.GOZGON_CITY, (RegionIdConst.NAVOI_REGION, "1712412", null, "G'ozg'on shahri", false) },

            // Namangan viloyati (14 ta)
            { DistrictIdConst.KOSONSOY, (RegionIdConst.NAMANGAN_REGION, "1714207", "1408", "Kosonsoy", false) },
            { DistrictIdConst.MINGBULOQ, (RegionIdConst.NAMANGAN_REGION, "1714204", "1407", "Mingbuloq", false) },
            { DistrictIdConst.NAMANGAN_D, (RegionIdConst.NAMANGAN_REGION, "1714212", "1409", "Namangan", false) },
            { DistrictIdConst.NORIN, (RegionIdConst.NAMANGAN_REGION, "1714216", "1410", "Norin", false) },
            { DistrictIdConst.POP, (RegionIdConst.NAMANGAN_REGION, "1714219", "1411", "Pop", false) },
            { DistrictIdConst.TORAQORGON, (RegionIdConst.NAMANGAN_REGION, "1714224", "1412", "To'raqo'rg'on", false) },
            { DistrictIdConst.UYCHI, (RegionIdConst.NAMANGAN_REGION, "1714229", "1413", "Uychi", false) },
            { DistrictIdConst.UCHQORGON, (RegionIdConst.NAMANGAN_REGION, "1714234", "1414", "Uchqo'rg'on", false) },
            { DistrictIdConst.CHORTOQ, (RegionIdConst.NAMANGAN_REGION, "1714236", "1415", "Chortoq", false) },
            { DistrictIdConst.CHUST, (RegionIdConst.NAMANGAN_REGION, "1714237", "1416", "Chust", false) },
            { DistrictIdConst.YANGIQORGON, (RegionIdConst.NAMANGAN_REGION, "1714242", "1417", "Yangiqo'rg'on", false) },
            { DistrictIdConst.NAMANGAN_CITY, (RegionIdConst.NAMANGAN_REGION, "1714401", "1401", "Namangan shahri", true) },
            { DistrictIdConst.DAVLATOBOD, (RegionIdConst.NAMANGAN_REGION, "1714401365", null, "Davlatobod", false) },
            { DistrictIdConst.YANGI_NAMANGAN, (RegionIdConst.NAMANGAN_REGION, "1714401367", null, "Yangi Namangan", false) },

            // Samarqand viloyati (16 ta)
            { DistrictIdConst.OQDARYO, (RegionIdConst.SAMARKAND_REGION, "1718203", "1808", "Oqdaryo", false) },
            { DistrictIdConst.BULUNGUR, (RegionIdConst.SAMARKAND_REGION, "1718206", "1809", "Bulung'ur", false) },
            { DistrictIdConst.JOMBOY, (RegionIdConst.SAMARKAND_REGION, "1718209", "1811", "Jomboy", false) },
            { DistrictIdConst.ISHTIXON, (RegionIdConst.SAMARKAND_REGION, "1718212", "1812", "Ishtixon", false) },
            { DistrictIdConst.KATTAQORGON_D, (RegionIdConst.SAMARKAND_REGION, "1718215", "1813", "Kattaqo'rg'on tumani", false) },
            { DistrictIdConst.QOSHROBOD, (RegionIdConst.SAMARKAND_REGION, "1718216", "1814", "Qo'shrobod", false) },
            { DistrictIdConst.NARPAY, (RegionIdConst.SAMARKAND_REGION, "1718218", "1815", "Narpay", false) },
            { DistrictIdConst.NUROBOD, (RegionIdConst.SAMARKAND_REGION, "1718235", "1821", "Nurobod", false) },
            { DistrictIdConst.PAYARIQ, (RegionIdConst.SAMARKAND_REGION, "1718224", "1817", "Payariq", false) },
            { DistrictIdConst.PASTDARGOM, (RegionIdConst.SAMARKAND_REGION, "1718227", "1818", "Pastdarg'om", false) },
            { DistrictIdConst.PAXTACHI, (RegionIdConst.SAMARKAND_REGION, "1718230", "1819", "Paxtachi", false) },
            { DistrictIdConst.SAMARQAND_D, (RegionIdConst.SAMARKAND_REGION, "1718233", "1820", "Samarqand", false) },
            { DistrictIdConst.TOYLOQ, (RegionIdConst.SAMARKAND_REGION, "1718238", "1804", "Toyloq", false) },
            { DistrictIdConst.URGUT, (RegionIdConst.SAMARKAND_REGION, "1718236", "1822", "Urgut", false) },
            { DistrictIdConst.KATTAQORGON_CITY, (RegionIdConst.SAMARKAND_REGION, "1718406", "1806", "Kattaqo'rg'on shahri", false) },
            { DistrictIdConst.SAMARQAND_CITY, (RegionIdConst.SAMARKAND_REGION, "1718401", "1801", "Samarqand shahri", true) },

            // Surxondaryo viloyati (15 ta)
            { DistrictIdConst.OLTINSOY, (RegionIdConst.SURKHANDARYA_REGION, "1722201", "2204", "Oltinsoy", false) },
            { DistrictIdConst.ANGOR, (RegionIdConst.SURKHANDARYA_REGION, "1722202", "2203", "Angor", false) },
            { DistrictIdConst.BOYSUN, (RegionIdConst.SURKHANDARYA_REGION, "1722204", "2205", "Boysun", false) },
            { DistrictIdConst.BANDIXON, (RegionIdConst.SURKHANDARYA_REGION, "1722203", "2216", "Bandixon", false) },
            { DistrictIdConst.DENOV, (RegionIdConst.SURKHANDARYA_REGION, "1722210", "2207", "Denov", false) },
            { DistrictIdConst.JARQORGON, (RegionIdConst.SURKHANDARYA_REGION, "1722212", "2208", "Jarqo'rg'on", false) },
            { DistrictIdConst.QUMQORGON, (RegionIdConst.SURKHANDARYA_REGION, "1722214", "2209", "Qumqo'rg'on", false) },
            { DistrictIdConst.QIZIRIQ, (RegionIdConst.SURKHANDARYA_REGION, "1722215", "2210", "Qiziriq", false) },
            { DistrictIdConst.MUZROBOD, (RegionIdConst.SURKHANDARYA_REGION, "1722207", "2206", "Muzrobod", false) },
            { DistrictIdConst.SARIOSIYO, (RegionIdConst.SURKHANDARYA_REGION, "1722217", "2211", "Sariosiyo", false) },
            { DistrictIdConst.TERMIZ_D, (RegionIdConst.SURKHANDARYA_REGION, "1722220", "2212", "Termiz", false) },
            { DistrictIdConst.UZUN, (RegionIdConst.SURKHANDARYA_REGION, "1722221", "2215", "Uzun", false) },
            { DistrictIdConst.SHEROBOD, (RegionIdConst.SURKHANDARYA_REGION, "1722223", "2213", "Sherobod", false) },
            { DistrictIdConst.SHORCHI, (RegionIdConst.SURKHANDARYA_REGION, "1722226", "2214", "Sho'rchi", false) },
            { DistrictIdConst.TERMIZ_CITY, (RegionIdConst.SURKHANDARYA_REGION, "1722401", "2201", "Termiz shahri", true) },

            // Sirdaryo viloyati (11 ta)
            { DistrictIdConst.OQOLTIN, (RegionIdConst.SYRDARYA_REGION, "1724206", "2401", "Oqoltin", false) },
            { DistrictIdConst.BOYOVUT, (RegionIdConst.SYRDARYA_REGION, "1724212", "2402", "Boyovut", false) },
            { DistrictIdConst.GULISTON_D, (RegionIdConst.SYRDARYA_REGION, "1724220", "2403", "Guliston", false) },
            { DistrictIdConst.MIRZAOBOD, (RegionIdConst.SYRDARYA_REGION, "1724228", "2405", "Mirzaobod", false) },
            { DistrictIdConst.SARDOBA, (RegionIdConst.SYRDARYA_REGION, "1724226", "2409", "Sardoba", false) },
            { DistrictIdConst.SAYXUNOBOD, (RegionIdConst.SYRDARYA_REGION, "1724216", "2406", "Sayxunobod", false) },
            { DistrictIdConst.SIRDARYO_D, (RegionIdConst.SYRDARYA_REGION, "1724231", "2407", "Sirdaryo", false) },
            { DistrictIdConst.XOVOS, (RegionIdConst.SYRDARYA_REGION, "1724235", "2408", "Xovos", false) },
            { DistrictIdConst.GULISTON_CITY, (RegionIdConst.SYRDARYA_REGION, "1724401", "2410", "Guliston shahri", true) },
            { DistrictIdConst.SHIRIN_CITY, (RegionIdConst.SYRDARYA_REGION, "1724410", "2412", "Shirin shahri", false) },
            { DistrictIdConst.YANGIER_CITY, (RegionIdConst.SYRDARYA_REGION, "1724413", "2413", "Yangier shahri", false) },

            // Farg'ona viloyati (19 ta)
            { DistrictIdConst.OLTIARIQ, (RegionIdConst.FERGHANA_REGION, "1730203", "3012", "Oltiariq", false) },
            { DistrictIdConst.QOSHTEPA, (RegionIdConst.FERGHANA_REGION, "1730206", "3013", "Qo'shtepa", false) },
            { DistrictIdConst.BOGDOD, (RegionIdConst.FERGHANA_REGION, "1730209", "3007", "Bog'dod", false) },
            { DistrictIdConst.BESHARIQ, (RegionIdConst.FERGHANA_REGION, "1730215", "3006", "Beshariq", false) },
            { DistrictIdConst.BUVAYDA, (RegionIdConst.FERGHANA_REGION, "1730212", "3008", "Buvayda", false) },
            { DistrictIdConst.DANGARA, (RegionIdConst.FERGHANA_REGION, "1730236", "3009", "Dang'ara", false) },
            { DistrictIdConst.QUVA, (RegionIdConst.FERGHANA_REGION, "1730218", "3011", "Quva", false) },
            { DistrictIdConst.RISHTON, (RegionIdConst.FERGHANA_REGION, "1730224", "3014", "Rishton", false) },
            { DistrictIdConst.SOX, (RegionIdConst.FERGHANA_REGION, "1730226", "3015", "So'x", false) },
            { DistrictIdConst.TOSHLOQ, (RegionIdConst.FERGHANA_REGION, "1730227", "3016", "Toshloq", false) },
            { DistrictIdConst.OZBEKISTON, (RegionIdConst.FERGHANA_REGION, "1730230", "3017", "O'zbekiston", false) },
            { DistrictIdConst.UCHKOPRIK, (RegionIdConst.FERGHANA_REGION, "1730221", "3018", "Uchko'prik", false) },
            { DistrictIdConst.FARGONA_D, (RegionIdConst.FERGHANA_REGION, "1730233", "3019", "Farg'ona", false) },
            { DistrictIdConst.FURQAT, (RegionIdConst.FERGHANA_REGION, "1730238", "3020", "Furqat", false) },
            { DistrictIdConst.YOZYOVON, (RegionIdConst.FERGHANA_REGION, "1730242", "3010", "Yozyovon", false) },
            { DistrictIdConst.QOQON_CITY, (RegionIdConst.FERGHANA_REGION, "1730405", "3003", "Qo'qon shahri", false) },
            { DistrictIdConst.QUVASOY_CITY, (RegionIdConst.FERGHANA_REGION, "1730408", "3001", "Quvasoy shahri", false) },
            { DistrictIdConst.MARGILON_CITY, (RegionIdConst.FERGHANA_REGION, "1730412", "3004", "Marg'ilon shahri", false) },
            { DistrictIdConst.FARGONA_CITY, (RegionIdConst.FERGHANA_REGION, "1730401", "3005", "Farg'ona shahri", true) },

            // Xorazm viloyati (13 ta)
            { DistrictIdConst.BOGOT, (RegionIdConst.KHOREZM_REGION, "1733204", "3311", "Bog'ot", false) },
            { DistrictIdConst.GURLAN, (RegionIdConst.KHOREZM_REGION, "1733208", "3307", "Gurlan", false) },
            { DistrictIdConst.QOSHKOPIR, (RegionIdConst.KHOREZM_REGION, "1733212", "3310", "Qoshko'pir", false) },
            { DistrictIdConst.URGANCH_CITY, (RegionIdConst.KHOREZM_REGION, "1733401", "3301", "Urganch shahri", true) },
            { DistrictIdConst.XAZORASP, (RegionIdConst.KHOREZM_REGION, "1733220", "3306", "Xazorasp", false) },
            { DistrictIdConst.XONQA, (RegionIdConst.KHOREZM_REGION, "1733223", "3312", "Xonqa", false) },
            { DistrictIdConst.XIVA_D, (RegionIdConst.KHOREZM_REGION, "1733226", "3305", "Xiva", false) },
            { DistrictIdConst.SHOVOT, (RegionIdConst.KHOREZM_REGION, "1733230", "3308", "Shovot", false) },
            { DistrictIdConst.YANGIARIQ, (RegionIdConst.KHOREZM_REGION, "1733233", "3309", "Yangiariq", false) },
            { DistrictIdConst.YANGIBOZOR, (RegionIdConst.KHOREZM_REGION, "1733236", "3313", "Yangibozor", false) },
            { DistrictIdConst.URGANCH_D, (RegionIdConst.KHOREZM_REGION, "1733217", "3304", "Urganch tumani", false) },
            { DistrictIdConst.XIVA_CITY, (RegionIdConst.KHOREZM_REGION, "1733406", null, "Xiva shahri", false) },
            { DistrictIdConst.TUPRAQQALA, (RegionIdConst.KHOREZM_REGION, "1733221", null, "Tupraqqal'a", false) }
        };

        return await SeedInfoEntitiesAsync(
            dbContext,
            logger,
            districtDefinitions,
            (id, data) =>
            {
                var district = CreateInfoEntity<InfoDistrict>(id);
                district.RegionId = data.RegionId;
                district.Code = GenerateCodeFromFullName(data.Name);
                district.Soato = data.Soato;
                district.RoamingCode = data.RoamingCode;
                district.ShortName = data.Name;
                district.FullName = data.Name;
                district.IsCenter = data.IsCenter;
                return district;
            },
            d => d.Code,
            "Districts"
        );
    }

    /// <summary>
    /// Generic method for seeding info entities with code-based uniqueness
    /// </summary>
    private static async Task<int> SeedInfoEntitiesAsync<TEntity, TData>(
        ApplicationDbContext dbContext,
        ILogger logger,
        Dictionary<int, TData> definitions,
        Func<int, TData, TEntity> entityFactory,
        Expression<Func<TEntity, string>> codeSelector,
        string entityName) where TEntity : class
    {
        var existingCodes = await dbContext.Set<TEntity>()
            .AsNoTracking()
            .Select(codeSelector)
            .ToListAsync();

        var existingCodesSet = existingCodes.ToHashSet();
        var codeSelectorFunc = codeSelector.Compile();

        var entitiesToAdd = new List<TEntity>();

        foreach (var (id, data) in definitions)
        {
            var entity = entityFactory(id, data);
            var code = codeSelectorFunc(entity);

            if (!existingCodesSet.Contains(code))
            {
                entitiesToAdd.Add(entity);
            }
        }

        // Check for unknown codes in database
        var definedCodes = definitions
            .Select(kvp => codeSelectorFunc(entityFactory(kvp.Key, kvp.Value)))
            .ToHashSet();

        var unknownCodes = existingCodesSet.Except(definedCodes).ToList();
        if (unknownCodes.Any())
        {
            logger.LogWarning(
                "Unknown {EntityName} codes found in database: {Codes}",
                entityName,
                string.Join(", ", unknownCodes));
        }

        if (entitiesToAdd.Count > 0)
        {
            await dbContext.Set<TEntity>().AddRangeAsync(entitiesToAdd);
            await dbContext.SaveChangesAsync();

            logger.LogInformation(
                "Added {Count} new {EntityName}: {Codes}",
                entitiesToAdd.Count,
                entityName,
                string.Join(", ", entitiesToAdd.Take(5).Select(codeSelectorFunc)) +
                (entitiesToAdd.Count > 5 ? $" and {entitiesToAdd.Count - 5} more" : ""));

            return entitiesToAdd.Count;
        }

        logger.LogDebug("No new {EntityName} to add", entityName);
        return 0;
    }

    /// <summary>
    /// Creates an info entity using its private constructor with ID parameter
    /// </summary>
    private static TEntity CreateInfoEntity<TEntity>(int id) where TEntity : class
    {
        var entity = (TEntity?)Activator.CreateInstance(
            typeof(TEntity),
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
            null,
            new object[] { id },
            null);

        if (entity == null)
        {
            throw new InvalidOperationException(
                $"Failed to create instance of {typeof(TEntity).Name} with id {id}");
        }

        return entity;
    }

    /// <summary>
    /// Generates a unique code from FullName by converting to uppercase and removing special characters
    /// </summary>
    private static string GenerateCodeFromFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("FullName cannot be null or empty", nameof(fullName));
        }

        var cleanedName = new StringBuilder();
        foreach (var c in fullName)
        {
            if (char.IsLetterOrDigit(c))
            {
                cleanedName.Append(char.ToUpperInvariant(c));
            }
        }

        var code = cleanedName.ToString();

        if (code.Length > 20)
        {
            code = code.Substring(0, 20);
        }

        if (string.IsNullOrEmpty(code))
        {
            code = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpperInvariant();
        }

        return code;
    }
}