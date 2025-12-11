using Domain.Abstraction.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public class Repository<TEntity> : IBaseRepository<TEntity> where TEntity : Entity
{
    protected readonly DbContext _context;
    protected readonly DbSet<TEntity> _dbSet;
    protected readonly ILogger<Repository<TEntity>> _logger;

    public Repository(
        DbContext context,
        ILogger<Repository<TEntity>> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<TEntity>();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public virtual IQueryable<TEntity> GetQueryable()
    {
        IQueryable<TEntity> query = _dbSet;

        // Automatic soft delete filter
        if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
        {
            query = query.Where(e => !((ISoftDelete)e).IsDeleted);
        }

        return query;
    }

    public virtual async Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity by id: {Id}", id);
            throw;
        }
    }

    public virtual async Task<TEntity?> GetByIdAsync(
        Guid id,
        params Expression<Func<TEntity, object>>[] includes)
    {
        try
        {
            IQueryable<TEntity> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(e => e.Id == id);
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
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(e => e.Id == id, cancellationToken);
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

    public virtual async Task<TEntity?> SingleOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable()
            .SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Set audit fields if applicable
            if (entity is IAuditable auditable && auditable.CreatedBy == null)
            {
                // You can get current user from IHttpContextAccessor here
            }

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
            // Set audit fields
            if (entity is AuditableEntity auditable)
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
                if (entity is AuditableEntity auditable)
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
            // Soft delete if supported
            if (entity is AuditableEntity auditable)
            {
                auditable.MarkAsDeleted();
                Update(entity);
            }
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
        Guid id,
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