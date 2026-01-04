using Domain.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class UserConfiguration : AuditableEntityConfiguration<User, Guid>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        // Primary SecretKey
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.PersonId)
            .IsRequired();

        builder.Property(x => x.UserName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Email)
            .HasMaxLength(100);

        builder.Property(x => x.IsEmailConfirmed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(x => x.UserName)
            .IsUnique()
            .HasDatabaseName("IX_Users_UserName");

        builder.HasIndex(x => x.Email)
            .HasDatabaseName("IX_Users_Email");

        builder.HasIndex(x => x.PersonId)
            .IsUnique()
            .HasDatabaseName("IX_Users_PersonId");

        // Relationships
        builder.HasOne(x => x.Person)
            .WithOne()
            .HasForeignKey<User>(x => x.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}