using Domain.Models.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class AuthTypeConfiguration : AuditableEntityConfiguration<EnumAuthType, int>
{
    public override void Configure(EntityTypeBuilder<EnumAuthType> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(25);

        builder.Property(x => x.ShortName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Code)
            .IsUnique();
    }
}