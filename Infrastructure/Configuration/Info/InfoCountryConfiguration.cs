using Domain.EfClasses.Info;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class InfoCountryConfiguration : AuditableEntityConfiguration<InfoCountry, int>
{
    public override void Configure(EntityTypeBuilder<InfoCountry> builder)
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

        builder.Property(x => x.ShortName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(200);

        // Indexes
        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("IX_InfoCountries_Code");

        builder.HasIndex(x => x.ShortName)
            .HasDatabaseName("IX_InfoCountries_ShortName");

        // Relationships
        builder.HasMany(x => x.Regions)
            .WithOne(x => x.Country)
            .HasForeignKey(x => x.CountryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}