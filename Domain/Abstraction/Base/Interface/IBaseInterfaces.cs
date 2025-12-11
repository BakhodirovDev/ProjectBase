using Domain.Abstraction.Results;

namespace Domain.Abstraction.Base;


/// <summary>
/// Base read operations - Never modify this interface
/// </summary>
public interface IReadOperations<TGetDto, TListDto, TPaginationOptions>
    where TPaginationOptions : PaginationParameters
{
    Task<Result<TGetDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PaginatedResult<TListDto>>> GetListAsync(TPaginationOptions paginationParams, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<int>> CountAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Base write operations - Never modify this interface
/// </summary>
public interface IWriteOperations<TGetDto, TCreateDto, TUpdateDto>
{
    Task<Result<TGetDto>> CreateAsync(TCreateDto createDto, CancellationToken cancellationToken = default);
    Task<Result<TGetDto>> UpdateAsync(Guid id, TUpdateDto updateDto, CancellationToken cancellationToken = default);
}

/// <summary>
/// Base delete operations - Never modify this interface
/// </summary>
public interface IDeleteOperations
{
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<bool>> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<bool>> RestoreAsync(Guid id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Base select list operations - Never modify this interface
/// </summary>
public interface ISelectListOperations
{
    Task<Result<IEnumerable<SelectListItem>>> GetSelectListAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<SelectListItem>>> GetSelectListAsync(string? searchTerm, int? limit = null, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<SelectListItem>>> GetActiveSelectListAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Batch operations - Never modify this interface
/// </summary>
public interface IBatchOperations<TGetDto, TCreateDto, TUpdateDto>
{
    Task<Result<IEnumerable<TGetDto>>> CreateManyAsync(IEnumerable<TCreateDto> createDtos, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<TGetDto>>> UpdateManyAsync(IDictionary<Guid, TUpdateDto> updates, CancellationToken cancellationToken = default);
    Task<Result<int>> DeleteManyAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}

/// <summary>
/// Search and filter operations - Never modify this interface
/// </summary>
public interface ISearchOperations<TListDto, TFilter> where TFilter : class
{
    Task<Result<PaginatedResult<TListDto>>> SearchAsync(TFilter filter, PaginationParameters pagination, CancellationToken cancellationToken = default);
}

// ============================================================================
// COMBINED INTERFACES
// ============================================================================

/// <summary>
/// Complete CRUD service interface - Extensible, never modify
/// </summary>
public interface ICrudService<TGetDto, TListDto, TCreateDto, TUpdateDto, TPaginationOptions> :
    IReadOperations<TGetDto, TListDto, TPaginationOptions>,
    IWriteOperations<TGetDto, TCreateDto, TUpdateDto>,
    IDeleteOperations,
    ISelectListOperations
    where TPaginationOptions : PaginationParameters
{
}

/// <summary>
/// CRUD with default pagination
/// </summary>
public interface ICrudService<TGetDto, TListDto, TCreateDto, TUpdateDto> :
    ICrudService<TGetDto, TListDto, TCreateDto, TUpdateDto, PaginationParameters>
{
}

/// <summary>
/// Extended CRUD with batch operations
/// </summary>
public interface IExtendedCrudService<TGetDto, TListDto, TCreateDto, TUpdateDto, TPaginationOptions> :
    ICrudService<TGetDto, TListDto, TCreateDto, TUpdateDto, TPaginationOptions>,
    IBatchOperations<TGetDto, TCreateDto, TUpdateDto>
    where TPaginationOptions : PaginationParameters
{
}

/// <summary>
/// Extended CRUD with search
/// </summary>
public interface ISearchableCrudService<TGetDto, TListDto, TCreateDto, TUpdateDto, TFilter, TPaginationOptions> :
    ICrudService<TGetDto, TListDto, TCreateDto, TUpdateDto, TPaginationOptions>,
    ISearchOperations<TListDto, TFilter>
    where TFilter : class
    where TPaginationOptions : PaginationParameters
{
}

/// <summary>
/// Read-only service
/// </summary>
public interface IReadOnlyService<TGetDto, TListDto, TPaginationOptions> :
    IReadOperations<TGetDto, TListDto, TPaginationOptions>,
    ISelectListOperations
    where TPaginationOptions : PaginationParameters
{
}

/// <summary>
/// Lookup service (only select list)
/// </summary>
public interface ILookupService : ISelectListOperations
{
}
