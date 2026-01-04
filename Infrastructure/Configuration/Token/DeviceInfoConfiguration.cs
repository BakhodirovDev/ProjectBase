using Domain.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class DeviceInfoConfiguration : AuditableEntityConfiguration<DeviceInfo, Guid>
{
    public override void Configure(EntityTypeBuilder<DeviceInfo> builder)
    {
        base.Configure(builder);
         
        builder.HasKey(x => x.Id);

        // IP Address - IPv4 (15 char) va IPv6 (45 char) uchun
        builder.Property(x => x.IpAddress)
            .IsRequired()
            .HasMaxLength(45);

        // Device Type
        builder.Property(x => x.DeviceType)
            .IsRequired()
            .HasMaxLength(50);

        // Device Model - uzunroq modellar uchun
        builder.Property(x => x.DeviceModel)
            .IsRequired()
            .HasMaxLength(200);

        // Operating System Name
        builder.Property(x => x.OsName)
            .IsRequired()
            .HasMaxLength(50);

        // Operating System Version
        builder.Property(x => x.OsVersion)
            .IsRequired()
            .HasMaxLength(50);

        // Browser Name
        builder.Property(x => x.BrowserName)
            .IsRequired()
            .HasMaxLength(50);

        // Browser Version
        builder.Property(x => x.BrowserVersion)
            .IsRequired()
            .HasMaxLength(50);

        // UserAgent - to'liq string (juda uzun bo'lishi mumkin)
        builder.Property(x => x.UserAgent)
            .IsRequired()
            .HasMaxLength(1000);

        // Boolean fields
        builder.Property(x => x.IsBot)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.IsMobile)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.IsTablet)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.IsDesktop)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.IsTrusted)
            .IsRequired()
            .HasDefaultValue(false);

        // Optional fields
        builder.Property(x => x.Location)
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(x => x.CountryCode)
            .HasMaxLength(2)
            .IsRequired(false)
            .IsFixedLength();

        builder.Property(x => x.DeviceNickname)
            .HasMaxLength(100)
            .IsRequired(false);

        // DateTime fields
        builder.Property(x => x.LastActivityAt)
            .IsRequired(false);

        // Counter field
        builder.Property(x => x.LoginCount)
            .IsRequired()
            .HasDefaultValue(1);

        // Foreign SecretKey
        builder.Property(x => x.TokenId)
            .IsRequired();

        // Relationships
        builder.HasOne(x => x.Token)
            .WithMany()
            .HasForeignKey(x => x.TokenId)
            .OnDelete(DeleteBehavior.Cascade);

        // IP Address index - tez-tez qidiriladi
        builder.HasIndex(x => x.IpAddress)
            .HasDatabaseName("IX_DeviceInfos_IpAddress")
            .HasFilter(null);

        // TokenId index - har bir token uchun device'lar
        builder.HasIndex(x => x.TokenId)
            .HasDatabaseName("IX_DeviceInfos_TokenId");

        // Composite index - device type va OS query'lari uchun
        builder.HasIndex(x => new { x.DeviceType, x.OsName })
            .HasDatabaseName("IX_DeviceInfos_DeviceType_OsName");

        // Bot filter index - botlarni tezkor filtrlash uchun
        builder.HasIndex(x => x.IsBot)
            .HasDatabaseName("IX_DeviceInfos_IsBot")
            .HasFilter("\"is_bot\" = true"); // Faqat bot=true bo'lganlar uchun

        // Trusted devices index - security check'lar uchun
        builder.HasIndex(x => x.IsTrusted)
            .HasDatabaseName("IX_DeviceInfos_IsTrusted");

        // Last activity index - faol qurilmalarni topish uchun
        builder.HasIndex(x => x.LastActivityAt)
            .HasDatabaseName("IX_DeviceInfos_LastActivityAt")
            .IsDescending();

        // Country code index - geografik statistika uchun
        builder.HasIndex(x => x.CountryCode)
            .HasDatabaseName("IX_DeviceInfos_CountryCode")
            .HasFilter("\"country_code\" IS NOT NULL");

        // Composite index - security monitoring uchun
        builder.HasIndex(x => new { x.IpAddress, x.LastActivityAt })
            .HasDatabaseName("IX_DeviceInfos_IpAddress_LastActivityAt");

        // Browser statistics uchun
        builder.HasIndex(x => new { x.BrowserName, x.BrowserVersion })
            .HasDatabaseName("IX_DeviceInfos_Browser");

        // Mobile/Tablet/Desktop filtering uchun
        builder.HasIndex(x => new { x.IsMobile, x.IsTablet, x.IsDesktop })
            .HasDatabaseName("IX_DeviceInfos_DeviceFlags");
    }
}
