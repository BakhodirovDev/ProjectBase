using Domain.EfClasses.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class PermissionConfiguration : AuditableEntityConfiguration<Permission, Guid>
{
    public override void Configure(EntityTypeBuilder<Permission> builder)
    {
        base.Configure(builder);

        // Primary SecretKey
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Resource)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Action)
            .IsRequired()
            .HasMaxLength(50);

        // Indexes
        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("IX_Permissions_Name");

        builder.HasIndex(x => new { x.Resource, x.Action })
            .IsUnique()
            .HasDatabaseName("IX_Permissions_Resource_Action");

        builder.HasIndex(x => x.Resource)
            .HasDatabaseName("IX_Permissions_Resource");
    }
}