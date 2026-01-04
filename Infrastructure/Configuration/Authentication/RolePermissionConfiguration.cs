using Domain.EfClasses.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class RolePermissionConfiguration : AuditableEntityConfiguration<RolePermission,Guid>
{
    public override void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        base.Configure(builder);

        // Primary SecretKey
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.RoleId)
            .IsRequired();

        builder.Property(x => x.PermissionId)
            .IsRequired();

        // Indexes
        builder.HasIndex(x => x.RoleId)
            .HasDatabaseName("IX_RolePermissions_RoleId");

        builder.HasIndex(x => x.PermissionId)
            .HasDatabaseName("IX_RolePermissions_PermissionId");

        builder.HasIndex(x => new { x.RoleId, x.PermissionId })
            .IsUnique()
            .HasDatabaseName("IX_RolePermissions_RoleId_PermissionId");

        // Relationships
        builder.HasOne(x => x.Role)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Permission)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}