using Domain.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class TokenConfiguration : AuditableEntityConfiguration<Token, Guid>
{
    public override void Configure(EntityTypeBuilder<Token> builder)
    {
        base.Configure(builder);

        // Primary SecretKey
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.AccessTokenId)
            .IsRequired();

        builder.Property(x => x.AccessTokenExpireAt)
            .IsRequired();

        builder.Property(x => x.RefreshTokenId)
            .IsRequired();

        builder.Property(x => x.RefreshTokenExpireAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_Tokens_UserId");

        builder.HasIndex(x => x.AccessTokenId)
            .IsUnique()
            .HasDatabaseName("IX_Tokens_AccessTokenId");

        builder.HasIndex(x => x.RefreshTokenId)
            .IsUnique()
            .HasDatabaseName("IX_Tokens_RefreshTokenId");

        // Relationships
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DeviceInfo)
            .WithOne(d => d.Token)
            .HasForeignKey<DeviceInfo>(d => d.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
