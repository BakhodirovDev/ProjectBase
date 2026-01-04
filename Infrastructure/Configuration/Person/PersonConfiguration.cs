using Domain.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

/// <summary>
/// Person entity configuration
/// </summary>
public class PersonConfiguration : AuditableEntityConfiguration<Person,Guid>
{
    public override void Configure(EntityTypeBuilder<Person> builder)
    {
        base.Configure(builder);

        // Primary SecretKey
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(25);

        builder.Property(x => x.MiddleName)
            .IsRequired()
            .HasMaxLength(25);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(25);

        builder.Property(x => x.ShortName)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(75);

        builder.Property(x => x.GenderId)
            .IsRequired();

        builder.Property(x => x.NationalityId)
            .IsRequired();

        builder.Property(x => x.CountryId)
            .IsRequired();

        builder.Property(x => x.RegionId)
            .IsRequired();

        builder.Property(x => x.DistrictId)
            .IsRequired();

        builder.Property(x => x.MfyId)
            .IsRequired(false);

        builder.Property(x => x.Address)
            .HasMaxLength(500);

        builder.Property(x => x.PhotoId)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(x => x.FullName)
            .HasDatabaseName("IX_Persons_FullName");

        builder.HasIndex(x => new { x.FirstName, x.LastName })
            .HasDatabaseName("IX_Persons_FirstName_LastName");

        builder.HasIndex(x => x.GenderId)
            .HasDatabaseName("IX_Persons_GenderId");

        builder.HasIndex(x => x.CountryId)
            .HasDatabaseName("IX_Persons_CountryId");

        builder.HasIndex(x => x.RegionId)
            .HasDatabaseName("IX_Persons_RegionId");

        builder.HasIndex(x => x.DistrictId)
            .HasDatabaseName("IX_Persons_DistrictId");

        // Relationships
        builder.HasOne(x => x.Gender)
            .WithMany()
            .HasForeignKey(x => x.GenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Nationality)
            .WithMany()
            .HasForeignKey(x => x.NationalityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Country)
            .WithMany()
            .HasForeignKey(x => x.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Region)
            .WithMany()
            .HasForeignKey(x => x.RegionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.District)
            .WithMany()
            .HasForeignKey(x => x.DistrictId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Mfy)
            .WithMany()
            .HasForeignKey(x => x.MfyId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasMany(x => x.PhoneNumbers)
            .WithOne()
            .HasForeignKey(x => x.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}