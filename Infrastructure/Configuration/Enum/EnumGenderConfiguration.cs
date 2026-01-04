using Domain.Models.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class EnumGenderConfiguration : AuditableEntityConfiguration<EnumGender, int>
{
    public override void Configure(EntityTypeBuilder<EnumGender> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(10);

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
