using Domain.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

/// <summary>
/// PersonPhoneNumbers entity configuration
/// </summary>
public class PersonPhoneNumbersConfiguration : AuditableEntityConfiguration<PersonPhoneNumbers, Guid>
{
    public override void Configure(EntityTypeBuilder<PersonPhoneNumbers> builder)
    {
        base.Configure(builder);

        // Primary SecretKey
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.PersonId)
            .IsRequired();

        builder.Property(x => x.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        // Indexes
        builder.HasIndex(x => x.PersonId)
            .HasDatabaseName("IX_PersonPhoneNumbers_PersonId");

        builder.HasIndex(x => x.PhoneNumber)
            .HasDatabaseName("IX_PersonPhoneNumbers_PhoneNumber");

        builder.HasIndex(x => new { x.PersonId, x.PhoneNumber })
            .IsUnique()
            .HasDatabaseName("IX_PersonPhoneNumbers_PersonId_PhoneNumber");

        builder.HasOne<Person>()
            .WithMany(p => p.PhoneNumbers)
            .HasForeignKey(x => x.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}