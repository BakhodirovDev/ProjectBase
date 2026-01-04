using Domain.EfClasses.Info;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class InfoRegionConfiguration : AuditableEntityConfiguration<InfoRegion, int>
{
    public override void Configure(EntityTypeBuilder<InfoRegion> builder)
    {
        base.Configure(builder);

        // Primary SecretKey
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.Soato)
            .HasMaxLength(20);

        builder.Property(x => x.RoamingCode)
            .HasMaxLength(10);

        builder.Property(x => x.ShortName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(200);

        // Foreign Keys
        builder.Property(x => x.CountryId)
            .IsRequired();

        // Indexes
        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("IX_InfoRegions_Code");

        builder.HasIndex(x => x.CountryId)
            .HasDatabaseName("IX_InfoRegions_CountryId");

        builder.HasIndex(x => x.ShortName)
            .HasDatabaseName("IX_InfoRegions_ShortName");

        // Relationships
        builder.HasOne(x => x.Country)
            .WithMany(x => x.Regions)
            .HasForeignKey(x => x.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Districts)
            .WithOne(x => x.Region)
            .HasForeignKey(x => x.RegionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Mfys)
            .WithOne(x => x.Region)
            .HasForeignKey(x => x.RegionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
