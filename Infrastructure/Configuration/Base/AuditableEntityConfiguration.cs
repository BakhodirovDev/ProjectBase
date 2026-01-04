using Consts;
using Domain.Abstraction.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public abstract class AuditableEntityConfiguration<TEntity, TId> : IEntityTypeConfiguration<TEntity>
where TEntity : AuditableEntity<TId>
where TId : notnull
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.Property(x => x.StatusId)
            .IsRequired()
            .HasDefaultValue(StatusIdConst.CREATED);

        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => x.StatusId != StatusIdConst.DELETED);
    }
}