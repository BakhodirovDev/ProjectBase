namespace Domain.Abstraction.Base;

public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets repository for specific entity type
    /// </summary>
    IBaseRepository<TEntity> Repository<TEntity>() where TEntity : Entity;

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