using Application.Service.BaseService;
using AutoMapper;
using Domain.Abstraction.Base;
using Domain.Abstraction.Errors;
using Domain.Abstraction.Results;
using Domain.EfClasses.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Application.Service;

public class PermissionService : CrudService<Permission, PermissionDto, PermissionDto, CreatePermissionDto, UpdatePermissionDto, PaginationParameters>, IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;

    public PermissionService(
        IPermissionRepository permissionRepository,
        IMapper mapper,
        ILogger<PermissionService> logger,
        IUnitOfWork unitOfWork)
        : base(permissionRepository, mapper, logger, unitOfWork)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<Result<List<PermissionDto>>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default)
    {
        try
        {
            var permissions = await Repository.GetQueryable()
                .Where(p => ids.Contains(p.Id))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var dtos = Mapper.Map<List<PermissionDto>>(permissions);
            return Result<List<PermissionDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in GetByIdsAsync");
            return Result<List<PermissionDto>>.Failure(Error.FromException(ex));
        }
    }

    public async Task<Result<List<PermissionDto>>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var permissions = await _permissionRepository.GetByRoleIdAsync(roleId, cancellationToken);
            var dtos = Mapper.Map<List<PermissionDto>>(permissions);
            return Result<List<PermissionDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in GetByRoleIdAsync for roleId: {RoleId}", roleId);
            return Result<List<PermissionDto>>.Failure(Error.FromException(ex));
        }
    }

    public async Task<Result<List<PermissionDto>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var permissions = await _permissionRepository.GetByUserIdAsync(userId, cancellationToken);
            var dtos = Mapper.Map<List<PermissionDto>>(permissions);
            return Result<List<PermissionDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in GetByUserIdAsync for userId: {UserId}", userId);
            return Result<List<PermissionDto>>.Failure(Error.FromException(ex));
        }
    }

    public async Task<Result<PermissionDto>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            var permission = await _permissionRepository.GetByNameAsync(name, cancellationToken);

            if (permission == null)
                return Result<PermissionDto>.Failure(Error.Custom(ErrorCodes.NotFound, $"Permission with name '{name}' not found", System.Net.HttpStatusCode.NotFound));

            var dto = Mapper.Map<PermissionDto>(permission);
            return Result<PermissionDto>.Success(dto);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in GetByNameAsync for name: {Name}", name);
            return Result<PermissionDto>.Failure(Error.FromException(ex));
        }
    }

    public async Task<Result<List<PermissionDto>>> GetByResourceAsync(string resource, CancellationToken cancellationToken = default)
    {
        try
        {
            var permissions = await _permissionRepository.GetByResourceAsync(resource, cancellationToken);
            var dtos = Mapper.Map<List<PermissionDto>>(permissions);
            return Result<List<PermissionDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in GetByResourceAsync for resource: {Resource}", resource);
            return Result<List<PermissionDto>>.Failure(Error.FromException(ex));
        }
    }

    public async Task<Result<bool>> UserHasPermissionAsync(Guid userId, string permissionName, CancellationToken cancellationToken = default)
    {
        try
        {
            var hasPermission = await _permissionRepository.UserHasPermissionAsync(userId, permissionName, cancellationToken);
            return Result<bool>.Success(hasPermission);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in UserHasPermissionAsync for userId: {UserId}, permission: {Permission}", userId, permissionName);
            return Result<bool>.Failure(Error.FromException(ex));
        }
    }

    public async Task<Result<bool>> UserHasAllPermissionsAsync(Guid userId, IEnumerable<string> permissionNames, CancellationToken cancellationToken = default)
    {
        try
        {
            var userPermissions = await _permissionRepository.GetByUserIdAsync(userId, cancellationToken);
            var userPermissionNames = userPermissions.Select(p => p.Name.ToLowerInvariant()).ToHashSet();

            var hasAll = permissionNames.All(p => userPermissionNames.Contains(p.ToLowerInvariant()));
            return Result<bool>.Success(hasAll);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in UserHasAllPermissionsAsync for userId: {UserId}", userId);
            return Result<bool>.Failure(Error.FromException(ex));
        }
    }

    public async Task<Result<bool>> UserHasAnyPermissionAsync(Guid userId, IEnumerable<string> permissionNames, CancellationToken cancellationToken = default)
    {
        try
        {
            var userPermissions = await _permissionRepository.GetByUserIdAsync(userId, cancellationToken);
            var userPermissionNames = userPermissions.Select(p => p.Name.ToLowerInvariant()).ToHashSet();

            var hasAny = permissionNames.Any(p => userPermissionNames.Contains(p.ToLowerInvariant()));
            return Result<bool>.Success(hasAny);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in UserHasAnyPermissionAsync for userId: {UserId}", userId);
            return Result<bool>.Failure(Error.FromException(ex));
        }
    }

    protected override Expression<Func<Permission, SelectListItem>> GetSelectListProjection()
    {
        return p => new SelectListItem
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description
        };
    }

    protected override IQueryable<Permission> ApplySelectListSearch(IQueryable<Permission> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        var term = searchTerm.ToLowerInvariant();
        return query.Where(p =>
            p.Name.ToLower().Contains(term) ||
            (p.Description != null && p.Description.ToLower().Contains(term)) ||
            (p.Resource != null && p.Resource.ToLower().Contains(term)));
    }
}