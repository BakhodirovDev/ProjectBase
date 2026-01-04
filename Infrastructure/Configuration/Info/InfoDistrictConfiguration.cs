using Domain.EfClasses.Info;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class InfoDistrictConfiguration : AuditableEntityConfiguration<InfoDistrict, int>
{
    public override void Configure(EntityTypeBuilder<InfoDistrict> builder)
    {
        base.Configure(builder);

        // Primary SecretKey
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Soato)
            .IsRequired(false)
            .HasMaxLength(20);

        builder.Property(x => x.RoamingCode)
            .IsRequired(false)
            .HasMaxLength(10);

        builder.Property(x => x.ShortName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(200);

        // Foreign Keys
        builder.Property(x => x.RegionId)
            .IsRequired();

        // Indexes
        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("IX_InfoDistricts_Code");

        builder.HasIndex(x => x.RegionId)
            .HasDatabaseName("IX_InfoDistricts_RegionId");

        builder.HasIndex(x => x.ShortName)
            .HasDatabaseName("IX_InfoDistricts_ShortName");

        builder.HasIndex(x => new { x.Soato, x.RoamingCode })
            .HasDatabaseName("IX_InfoDistricts_Soato_RoamingCode");

        // Relationships
        builder.HasOne(x => x.Region)
            .WithMany(x => x.Districts)
            .HasForeignKey(x => x.RegionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Mfys)
            .WithOne(x => x.District)
            .HasForeignKey(x => x.DistrictId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}