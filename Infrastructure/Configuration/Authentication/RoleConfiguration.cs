using Domain.EfClasses.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class RoleConfiguration : AuditableEntityConfiguration<Role,Guid>
{
    public override void Configure(EntityTypeBuilder<Role> builder)
    {
        base.Configure(builder);

        // Primary SecretKey
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.IsDefault)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.IsSystemRole)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.IsSystemAdmin)
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("IX_Roles_Name");

        builder.HasIndex(x => x.IsSystemRole)
            .HasDatabaseName("IX_Roles_IsSystemRole");

        builder.HasIndex(x => x.IsSystemAdmin)
            .HasDatabaseName("IX_Roles_IsSystemAdmin");

        // Relationships
        builder.HasMany(x => x.RolePermissions)
            .WithOne(x => x.Role)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.UserRoles)
            .WithOne(x => x.Role)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}