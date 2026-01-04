namespace Domain.Abstraction.Base;

public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets repository for specific entity type with Guid Id
    /// </summary>
    IBaseRepository<TEntity> Repository<TEntity>()
        where TEntity : Entity<Guid>;

    /// <summary>
    /// Gets repository for specific entity type with custom Id type
    /// </summary>
    IBaseRepository<TEntity, TId> Repository<TEntity, TId>()
        where TEntity : Entity<TId>
        where TId : notnull;

    /// <summary>
    /// Saves all changes
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins transaction
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits transaction
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back transaction
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}