using System.Linq.Expressions;

namespace Domain.Abstraction.Base;

/// <summary>
/// Base repository interface for entity operations
/// Provides comprehensive data access methods following repository pattern
/// </summary>
public interface IBaseRepository<TEntity> where TEntity : Entity
{
    // ========================================================================
    // QUERY OPERATIONS
    // ========================================================================

    IQueryable<TEntity> GetQueryable();

    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TEntity?> GetByIdAsync(
        Guid id,
        params Expression<Func<TEntity, object>>[] includes);

    Task<List<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<(List<TEntity> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    // ========================================================================
    // WRITE OPERATIONS
    // ========================================================================

    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    void Update(TEntity entity);

    void UpdateRange(IEnumerable<TEntity> entities);

    void Delete(TEntity entity);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    void DeleteRange(IEnumerable<TEntity> entities);
}