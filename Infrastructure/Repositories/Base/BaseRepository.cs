using Consts;
using Domain.Abstraction.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation with custom Id type
/// </summary>
public class Repository<TEntity, TId> : IBaseRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    protected readonly DbContext _context;
    protected readonly DbSet<TEntity> _dbSet;
    protected readonly ILogger<Repository<TEntity, TId>> _logger;

    // Cache the type check results for performance
    private static readonly bool _isAuditableEntity = typeof(AuditableEntity<TId>).IsAssignableFrom(typeof(TEntity));
    private static readonly bool _isSoftDelete = typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity));

    public Repository(
        DbContext context,
        ILogger<Repository<TEntity, TId>> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<TEntity>();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public virtual IQueryable<TEntity> GetQueryable()
    {
        IQueryable<TEntity> query = _dbSet;

        // Automatic soft delete filter for AuditableEntity (StatusId based)
        if (_isAuditableEntity)
        {
            query = query.Where(e => ((AuditableEntity<TId>)(object)e).StatusId != StatusIdConst.DELETED);
        }
        // Fallback for ISoftDelete interface
        else if (_isSoftDelete)
        {
            query = query.Where(e => !((ISoftDelete)(object)e).IsDeleted);
        }

        return query;
    }

    public virtual async Task<TEntity?> GetByIdAsync(
        TId id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _dbSet.FindAsync(new object[] { id! }, cancellationToken);

            // Check if entity is soft deleted
            if (entity != null)
            {
                if (_isAuditableEntity && entity is AuditableEntity<TId> auditable && auditable.IsDeleted())
                    return null;
                if (_isSoftDelete && entity is ISoftDelete softDelete && softDelete.IsDeleted)
                    return null;
            }

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity by id: {Id}", id);
            throw;
        }
    }

    public virtual async Task<TEntity?> GetByIdAsync(
        TId id,
        params Expression<Func<TEntity, object>>[] includes)
    {
        try
        {
            IQueryable<TEntity> query = GetQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(e => e.Id.Equals(id));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity by id with includes: {Id}", id);
            throw;
        }
    }

    public virtual async Task<List<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetQueryable()
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding entities");
            throw;
        }
    }

    public virtual async Task<(List<TEntity> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<TEntity> query = GetQueryable();

            if (filter != null)
                query = query.Where(filter);

            var totalCount = await query.CountAsync(cancellationToken);

            if (orderBy != null)
                query = orderBy(query);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged entities");
            throw;
        }
    }

    public virtual async Task<bool> ExistsAsync(
        TId id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(e => e.Id.Equals(id), cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable().AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        var query = GetQueryable();

        if (predicate != null)
            query = query.Where(predicate);

        return await query.CountAsync(cancellationToken);
    }

    public virtual async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable()
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding entity");
            throw;
        }
    }

    public virtual async Task AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding entities");
            throw;
        }
    }

    public virtual void Update(TEntity entity)
    {
        try
        {
            // Set audit fields for AuditableEntity
            if (entity is AuditableEntity<TId> auditable)
            {
                auditable.MarkAsUpdated();
            }

            _dbSet.Update(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entity");
            throw;
        }
    }

    public virtual void UpdateRange(IEnumerable<TEntity> entities)
    {
        try
        {
            foreach (var entity in entities)
            {
                if (entity is AuditableEntity<TId> auditable)
                {
                    auditable.MarkAsUpdated();
                }
            }

            _dbSet.UpdateRange(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entities");
            throw;
        }
    }

    public virtual void Delete(TEntity entity)
    {
        try
        {
            // Soft delete for AuditableEntity (uses StatusId)
            if (entity is AuditableEntity<TId> auditable)
            {
                auditable.MarkAsDeleted();
                _dbSet.Update(entity);
            }
            // Soft delete for ISoftDelete interface
            else if (entity is ISoftDelete softDelete)
            {
                softDelete.IsDeleted = true;
                softDelete.DeletedAt = DateTime.UtcNow;
                _dbSet.Update(entity);
            }
            // Hard delete
            else
            {
                _dbSet.Remove(entity);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting entity");
            throw;
        }
    }

    public virtual async Task<bool> DeleteAsync(
        TId id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await GetByIdAsync(id, cancellationToken);

            if (entity == null)
                return false;

            Delete(entity);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting entity by id: {Id}", id);
            throw;
        }
    }

    public virtual void DeleteRange(IEnumerable<TEntity> entities)
    {
        try
        {
            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting entities");
            throw;
        }
    }
}

/// <summary>
/// Repository for entities with Guid Id (backward compatibility)
/// </summary>
public class Repository<TEntity> : Repository<TEntity, Guid>, IBaseRepository<TEntity>
    where TEntity : Entity<Guid>
{
    public Repository(
        DbContext context,
        ILogger<Repository<TEntity, Guid>> logger)
        : base(context, logger)
    {
    }
}
