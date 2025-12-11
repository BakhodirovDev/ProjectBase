using AutoMapper;
using Domain.Abstraction.Base;
using Domain.Abstraction.Errors;
using Domain.Abstraction.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;


namespace Application.Service.BaseService;

/// <summary>
/// Base CRUD service implementation - Production ready, extensible
/// NEVER MODIFY - Extend instead!
/// </summary>
public abstract class CrudServiceBase<TEntity, TGetDto, TListDto, TCreateDto, TUpdateDto, TPaginationOptions>
    : ICrudService<TGetDto, TListDto, TCreateDto, TUpdateDto, TPaginationOptions>
    where TEntity : Entity
    where TPaginationOptions : PaginationParameters
{
    protected readonly IBaseRepository<TEntity> Repository;
    protected readonly IMapper Mapper;
    protected readonly ILogger Logger;
    protected readonly IUnitOfWork UnitOfWork;

    protected CrudServiceBase(
        IBaseRepository<TEntity> repository,
        IMapper mapper,
        ILogger logger,
        IUnitOfWork unitOfWork)
    {
        Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    // ========================================================================
    // READ OPERATIONS
    // ========================================================================

    public virtual async Task<Result<TGetDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = Repository.GetQueryable();
            query = ApplyIncludes(query);

            var entity = await query
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (entity == null)
                return Result<TGetDto>.Failure(
                    Error.NotFoundWithEntity(typeof(TEntity).Name, id));

            if (!await CanReadAsync(entity, cancellationToken))
                return Result<TGetDto>.Failure(Error.Forbidden);

            var dto = Mapper.Map<TGetDto>(entity);
            return Result<TGetDto>.Success(dto);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in GetByIdAsync for id: {Id}", id);
            return Result<TGetDto>.Failure(Error.FromException(ex));
        }
    }

    public virtual async Task<Result<PaginatedResult<TListDto>>> GetListAsync(
        TPaginationOptions paginationParams,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = Repository.GetQueryable().AsNoTracking();

            // Apply filters
            query = ApplyBaseFilters(query);
            query = await ApplyCustomFiltersAsync(query, paginationParams, cancellationToken);

            // Get total count before pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply sorting
            query = ApplySorting(query, paginationParams);

            // Apply pagination
            query = query
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize);

            // Execute query
            var entities = await query.ToListAsync(cancellationToken);
            var dtos = Mapper.Map<List<TListDto>>(entities);

            var result = new PaginatedResult<TListDto>(
                dtos,
                totalCount,
                paginationParams.PageNumber,
                paginationParams.PageSize);

            return Result<PaginatedResult<TListDto>>.Success(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in GetListAsync");
            return Result<PaginatedResult<TListDto>>.Failure(Error.FromException(ex));
        }
    }

    public virtual async Task<Result<bool>> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var exists = await Repository
                .GetQueryable()
                .AnyAsync(e => e.Id == id, cancellationToken);

            return Result<bool>.Success(exists);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in ExistsAsync for id: {Id}", id);
            return Result<bool>.Failure(Error.FromException(ex));
        }
    }

    public virtual async Task<Result<int>> CountAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = Repository.GetQueryable();
            query = ApplyBaseFilters(query);

            var count = await query.CountAsync(cancellationToken);
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in CountAsync");
            return Result<int>.Failure(Error.FromException(ex));
        }
    }

    // ========================================================================
    // WRITE OPERATIONS
    // ========================================================================

    public virtual async Task<Result<TGetDto>> CreateAsync(
        TCreateDto createDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate
            var validationResult = await ValidateCreateAsync(createDto, cancellationToken);
            if (!validationResult.IsSuccess)
                return Result<TGetDto>.Failure(validationResult.Error);

            // Map to entity
            var entity = Mapper.Map<TEntity>(createDto);

            // Apply business rules
            await ApplyCreateBusinessRulesAsync(entity, createDto, cancellationToken);

            // Add to repository
            await Repository.AddAsync(entity, cancellationToken);

            // Post-create operations
            await OnEntityCreatedAsync(entity, cancellationToken);

            // Return result
            var dto = Mapper.Map<TGetDto>(entity);
            return Result<TGetDto>.Success(dto);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in CreateAsync");
            return Result<TGetDto>.Failure(Error.FromException(ex));
        }
    }

    public virtual async Task<Result<TGetDto>> UpdateAsync(
        Guid id,
        TUpdateDto updateDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Get existing entity
            var entity = await Repository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                return Result<TGetDto>.Failure(
                    Error.NotFoundWithEntity(typeof(TEntity).Name, id));

            // Check permissions
            if (!await CanUpdateAsync(entity, cancellationToken))
                return Result<TGetDto>.Failure(Error.Forbidden);

            // Validate
            var validationResult = await ValidateUpdateAsync(entity, updateDto, cancellationToken);
            if (!validationResult.IsSuccess)
                return Result<TGetDto>.Failure(validationResult.Error);

            // Map updates
            Mapper.Map(updateDto, entity);

            // Apply business rules
            await ApplyUpdateBusinessRulesAsync(entity, updateDto, cancellationToken);

            // Update
            Repository.Update(entity);

            // Post-update operations
            await OnEntityUpdatedAsync(entity, cancellationToken);

            // Return result
            var dto = Mapper.Map<TGetDto>(entity);
            return Result<TGetDto>.Success(dto);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in UpdateAsync for id: {Id}", id);
            return Result<TGetDto>.Failure(Error.FromException(ex));
        }
    }

    // ========================================================================
    // DELETE OPERATIONS
    // ========================================================================

    public virtual async Task<Result<bool>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await Repository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                return Result<bool>.Failure(
                    Error.NotFoundWithEntity(typeof(TEntity).Name, id));

            if (!await CanDeleteAsync(entity, cancellationToken))
                return Result<bool>.Failure(Error.Forbidden);

            Repository.Delete(entity);

            await OnEntityDeletedAsync(entity, cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in DeleteAsync for id: {Id}", id);
            return Result<bool>.Failure(Error.FromException(ex));
        }
    }

    public virtual async Task<Result<bool>> SoftDeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await Repository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                return Result<bool>.Failure(
                    Error.NotFoundWithEntity(typeof(TEntity).Name, id));

            if (entity is not ISoftDeletable softDeleteEntity)
                return await DeleteAsync(id, cancellationToken);

            if (!await CanDeleteAsync(entity, cancellationToken))
                return Result<bool>.Failure(Error.Forbidden);

            // Use proper method if AuditableEntity
            if (entity is AuditableEntity auditable)
            {
                auditable.MarkAsDeleted();
            }

            Repository.Update(entity);

            await OnEntitySoftDeletedAsync(entity, cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in SoftDeleteAsync for id: {Id}", id);
            return Result<bool>.Failure(Error.FromException(ex));
        }
    }

    public virtual async Task<Result<bool>> RestoreAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await Repository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                return Result<bool>.Failure(
                    Error.NotFoundWithEntity(typeof(TEntity).Name, id));

            if (entity is not ISoftDeletable)
                return Result<bool>.Failure(
                    Error.Custom("NOT_SUPPORTED", "Entity does not support soft delete"));

            // Use proper method if AuditableEntity
            if (entity is AuditableEntity auditable)
            {
                auditable.MarkAsRestored();
            }

            Repository.Update(entity);

            await OnEntityRestoredAsync(entity, cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in RestoreAsync for id: {Id}", id);
            return Result<bool>.Failure(Error.FromException(ex));
        }
    }

    // ========================================================================
    // SELECT LIST OPERATIONS
    // ========================================================================

    public virtual async Task<Result<IEnumerable<SelectListItem>>> GetSelectListAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = Repository.GetQueryable().AsNoTracking();
            query = ApplySelectListFilters(query);
            query = ApplySelectListOrdering(query);

            var items = await query
                .Select(GetSelectListProjection())
                .ToListAsync(cancellationToken);

            return Result<IEnumerable<SelectListItem>>.Success(items);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in GetSelectListAsync");
            return Result<IEnumerable<SelectListItem>>.Failure(Error.FromException(ex));
        }
    }

    public virtual async Task<Result<IEnumerable<SelectListItem>>> GetSelectListAsync(
        string? searchTerm,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = Repository.GetQueryable().AsNoTracking();
            query = ApplySelectListFilters(query);

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = ApplySelectListSearch(query, searchTerm);

            query = ApplySelectListOrdering(query);

            if (limit.HasValue && limit.Value > 0)
                query = query.Take(limit.Value);

            var items = await query
                .Select(GetSelectListProjection())
                .ToListAsync(cancellationToken);

            return Result<IEnumerable<SelectListItem>>.Success(items);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in GetSelectListAsync with search");
            return Result<IEnumerable<SelectListItem>>.Failure(Error.FromException(ex));
        }
    }

    public virtual async Task<Result<IEnumerable<SelectListItem>>> GetActiveSelectListAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = Repository.GetQueryable().AsNoTracking();
            query = ApplyActiveFilter(query);
            query = ApplySelectListOrdering(query);

            var items = await query
                .Select(GetSelectListProjection())
                .ToListAsync(cancellationToken);

            return Result<IEnumerable<SelectListItem>>.Success(items);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in GetActiveSelectListAsync");
            return Result<IEnumerable<SelectListItem>>.Failure(Error.FromException(ex));
        }
    }

    // ========================================================================
    // ABSTRACT METHODS
    // ========================================================================

    protected virtual IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query) => query;

    protected virtual IQueryable<TEntity> ApplyBaseFilters(IQueryable<TEntity> query)
    {
        if (typeof(ISoftDeletable).IsAssignableFrom(typeof(TEntity)))
            query = query.Where(e => !((ISoftDeletable)e).IsDeleted);

        return query;
    }

    protected virtual Task<IQueryable<TEntity>> ApplyCustomFiltersAsync(
        IQueryable<TEntity> query,
        TPaginationOptions paginationOptions,
        CancellationToken cancellationToken)
        => Task.FromResult(query);

    protected virtual IQueryable<TEntity> ApplySorting(
        IQueryable<TEntity> query,
        TPaginationOptions paginationOptions)
    {
        return query.OrderBy(e => e.Id);
    }

    protected abstract Expression<Func<TEntity, SelectListItem>> GetSelectListProjection();

    protected virtual IQueryable<TEntity> ApplySelectListFilters(IQueryable<TEntity> query)
    {
        return ApplyBaseFilters(query);
    }

    protected virtual IQueryable<TEntity> ApplySelectListOrdering(IQueryable<TEntity> query)
    {
        return query.OrderBy(e => e.Id);
    }

    protected abstract IQueryable<TEntity> ApplySelectListSearch(
        IQueryable<TEntity> query,
        string searchTerm);

    protected virtual IQueryable<TEntity> ApplyActiveFilter(IQueryable<TEntity> query)
    {
        if (typeof(IAuditableEntity).IsAssignableFrom(typeof(TEntity)))
        {
            // Assuming entities with IAuditableEntity also have IsActive
            // You might need to create IActivatable interface
        }

        return ApplyBaseFilters(query);
    }

    // ========================================================================
    // VALIDATION HOOKS
    // ========================================================================

    protected virtual Task<Result> ValidateCreateAsync(
        TCreateDto createDto,
        CancellationToken cancellationToken)
        => Task.FromResult(Result.Success());

    protected virtual Task<Result> ValidateUpdateAsync(
        TEntity entity,
        TUpdateDto updateDto,
        CancellationToken cancellationToken)
        => Task.FromResult(Result.Success());

    // ========================================================================
    // BUSINESS RULES HOOKS
    // ========================================================================

    protected virtual Task ApplyCreateBusinessRulesAsync(
        TEntity entity,
        TCreateDto createDto,
        CancellationToken cancellationToken)
        => Task.CompletedTask;

    protected virtual Task ApplyUpdateBusinessRulesAsync(
        TEntity entity,
        TUpdateDto updateDto,
        CancellationToken cancellationToken)
        => Task.CompletedTask;

    // ========================================================================
    // PERMISSION HOOKS
    // ========================================================================

    protected virtual Task<bool> CanReadAsync(
        TEntity entity,
        CancellationToken cancellationToken)
        => Task.FromResult(true);

    protected virtual Task<bool> CanUpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken)
        => Task.FromResult(true);

    protected virtual Task<bool> CanDeleteAsync(
        TEntity entity,
        CancellationToken cancellationToken)
        => Task.FromResult(true);

    // ========================================================================
    // EVENT HOOKS
    // ========================================================================

    protected virtual Task OnEntityCreatedAsync(
        TEntity entity,
        CancellationToken cancellationToken)
        => Task.CompletedTask;

    protected virtual Task OnEntityUpdatedAsync(
        TEntity entity,
        CancellationToken cancellationToken)
        => Task.CompletedTask;

    protected virtual Task OnEntityDeletedAsync(
        TEntity entity,
        CancellationToken cancellationToken)
        => Task.CompletedTask;

    protected virtual Task OnEntitySoftDeletedAsync(
        TEntity entity,
        CancellationToken cancellationToken)
        => Task.CompletedTask;

    protected virtual Task OnEntityRestoredAsync(
        TEntity entity,
        CancellationToken cancellationToken)
        => Task.CompletedTask;
}