using Domain.EfClasses.Info;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class InfoMfyConfiguration : AuditableEntityConfiguration<InfoMfy, int>
{
    public override void Configure(EntityTypeBuilder<InfoMfy> builder)
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

        builder.Property(x => x.ShortName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(200);

        // Foreign Keys
        builder.Property(x => x.RegionId)
            .IsRequired();

        builder.Property(x => x.DistrictId)
            .IsRequired();

        // Indexes
        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("IX_InfoMfys_Code");

        builder.HasIndex(x => x.RegionId)
            .HasDatabaseName("IX_InfoMfys_RegionId");

        builder.HasIndex(x => x.DistrictId)
            .HasDatabaseName("IX_InfoMfys_DistrictId");

        builder.HasIndex(x => new { x.RegionId, x.DistrictId })
            .HasDatabaseName("IX_InfoMfys_RegionId_DistrictId");

        builder.HasIndex(x => x.ShortName)
            .HasDatabaseName("IX_InfoMfys_ShortName");

        // Relationships
        builder.HasOne(x => x.Region)
            .WithMany(x => x.Mfys)
            .HasForeignKey(x => x.RegionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.District)
            .WithMany(x => x.Mfys)
            .HasForeignKey(x => x.DistrictId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}